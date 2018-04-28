using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using TAGGER.Utils;

namespace TAGGER.Commands
{
    public static class Calculate
    {
        private static readonly Regex Regex1 = new Regex(@"^(?:https?\:\/\/)?osu\.ppy\.sh\/(b|s)\/([0-9]+)(?:(?:\?|&)m=([0-3]))?$");
        private static readonly Regex Regex2 = new Regex(@"^(?:https?\:\/\/)?osu\.ppy\.sh\/p\/beatmap\?b=([0-9]+)(?:&m=([0-3]))?$");
        private static readonly Regex Regex3 = new Regex(@"^(?:https?\:\/\/)?osu\.ppy\.sh\/beatmapsets\/[0-9]+#(osu|taiko|fruits|mania)\/([0-9]+)$");
        private static readonly Regex Regex4 = new Regex(@"^(?:https?\:\/\/)?osu\.ppy\.sh\/beatmapsets\/[0-9]+/#(osu|taiko|fruits|mania)\/([0-9]+)$");
        /// <summary>
        /// Parse a beatmap-id and mode from an url
        /// </summary>
        /// <param name="url">The url te parse</param>
        /// <param name="beatmapId">The beatmap-id</param>
        /// <param name="mode">The mode or <code>null</code> if unknown</param>
        /// <returns><code>true</code> if the url contains a beatmap-id, <code>false</code> if it was a set-id, <code>null</code> for anything else</returns>
        public static bool? ParseBeatmapIdFromUrl(string url, out int beatmapId, out int? mode)
        {
            // try number-only
            if (int.TryParse(url, out beatmapId))
            {
                mode = null;
                return true;
            }

            // try url variant 1
            var result = Regex1.Match(url);
            if (result.Success)
            {
                if (result.Groups[1].Value[0] == 's')
                {
                    // its a set url
                    beatmapId = default(int);
                    mode = default(int?);
                    return false;
                }
                
                // its a beatmap-id url!
                beatmapId = int.Parse(result.Groups[2].Value);
                mode = GetModeFromResultGroup(result.Groups[3]);
                return true;
            }

            // try url variant 2
            result = Regex2.Match(url);
            if (result.Success)
            {
                beatmapId = int.Parse(result.Groups[2].Value);
                mode = GetModeFromResultGroup(result.Groups[2]);
                return true;
            }

            // try url variant 3
            result = Regex3.Match(url);
            if (result.Success)
            {
                beatmapId = int.Parse(result.Groups[2].Value);
                mode = GetNamedModeFromResultGroup(result.Groups[1]);
                return true;
            }

            // try url variant 4
            result = Regex4.Match(url);
            if (result.Success)
            {
                beatmapId = int.Parse(result.Groups[2].Value);
                mode = GetNamedModeFromResultGroup(result.Groups[1]);
                return true;
            }

            beatmapId = default(int);
            mode = default(int?);
            return null;
        }

        private static int? GetNamedModeFromResultGroup(Group resultGroup)
        {
            if (!resultGroup.Success) return null;
            switch (resultGroup.Value)
            {
                case "osu":
                    return 0;
                case "taiko":
                    return 1;
                case "fruits":
                    return 2;
                case "mania":
                    return 3;
                default:
                    return null;
            }
        }

        private static int? GetModeFromResultGroup(Group resultGroup)
        {
            if (!resultGroup.Success) return null;
            return int.Parse(resultGroup.Value);
        }

        private static async Task<bool> DownloadOsuFile(int beatmapId)
        {
            var url = $"https://osu.ppy.sh/osu/{beatmapId}";
            var fileLocation = $"Temp/{beatmapId}";
            var totalBytesTransfered = await Task.Run(() =>
            {
                if (!Directory.Exists("Temp"))
                    Directory.CreateDirectory("Temp");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var fileStream = File.Create(fileLocation))
                using (var responseStream = response.GetResponseStream())
                {
                    var buffer = new byte[32768];
                    var totalRead = 0;
                    int read;
                    while (responseStream != null && (read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalRead += read;
                        fileStream.Write(buffer, 0, read);
                    }
                    return totalRead;
                }
            });
            if (totalBytesTransfered == 0 || totalBytesTransfered == 1024*1024)
            {
                // nothing was in the response body, so asume beatmap doesnt exist
                // or beatmap was in bugged state, see https://github.com/ppy/osu-api/issues/131
                return true;
            }
            return true;
        }

        private static async Task<JObject> RunOppai(string name)
        {
            var info = new ProcessStartInfo
            {
                FileName = "oppai.exe",
                Arguments = $"Temp/{name} -ojson",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            var result = await Task.Run(() =>
            {
                using (var proc = new Process {StartInfo = info})
                {
                    proc.Start();
                    var stdout = proc.StandardOutput.ReadToEnd();
                    var stderr = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                    return Tuple.Create(proc.ExitCode, stdout, stderr);
                }
            });
            if (result.Item1 != 0)
            {
                // oppai failed
                throw new OppaiException(result.Item3.Trim());
            }
            return JObject.Parse(result.Item2);
        }

        public static async Task<double> Sr(CommandContext ctx, string link, int tag, bool web)
        {
            try
            {
                var isBeatmapId = ParseBeatmapIdFromUrl(link, out var beatmapId, out var mode);
                if (!isBeatmapId.HasValue)
                {
                    await
                        ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is not a beatmap or invalid.");
                    return 0.0;
                }

                if (!isBeatmapId.Value)
                {
                    await
                        ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is from a beatmap-set. Please click the name of the difficulty and provide the url again.");
                    return 0.0;
                }

                // filter on mode, only std allowed
                // if mode is unknown, asume std
                if (mode.HasValue && mode.Value != 0)
                {
                    // mode is given explicitly and not std
                    await ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is pointing to a non standard map.");
                    return 0.0;
                }

                var downloadSuccess = await DownloadOsuFile(beatmapId);
                if (!downloadSuccess)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} the beatmap could not be found on the osu server, or was in a bugged state on the osu server.");
                    return 0.0;
                }
                DiscordEmbedBuilder playerStars = null;
                double stars;
                if (tag > 1)
                {
                    playerStars = new DiscordEmbedBuilder();
                    SplitMap.Split(tag, beatmapId);
                    var starValues = new double[tag];
                    for (var i = 0; i < tag; i++)
                    {
                        var oppaiResult = await RunOppai($"{beatmapId}_{i}");
                        File.Delete($"Temp/{beatmapId}_{i}");
                        starValues[i] = Convert.ToDouble(oppaiResult.GetValue("stars"));
                        playerStars.AddField($"Player {i + 1}", $"{starValues[i]:0.##}", true);
                    }
                    File.Delete($"Temp/{beatmapId}");
                    stars = starValues.Average();
                    playerStars.WithFooter("Provided by http://tag.tayo.ws/");
                }
                else
                {
                    var oppaiResult = await RunOppai(beatmapId.ToString());
                    File.Delete($"Temp/{beatmapId}");
                    stars = Convert.ToDouble(oppaiResult.GetValue("stars"));
                }
                if (!web)
                {
                    await ctx.RespondAsync(
                        $"{ctx.Member.Mention} the amount of stars for the given beatmap with {tag} player(s) is {stars:0.##}*",
                        false, playerStars);
                }
                return Math.Round(stars, 2);
            }
            catch (OppaiException e)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} oppai could not parse the beatmap :cry: ({e.Message})");
                return 0.0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await ctx.RespondAsync($"{ctx.Member.Mention} something went wrong :cry:");
                return 0.0;
            }
        }
    }

    internal class OppaiException : Exception
    {
        public OppaiException()
        {
        }

        public OppaiException(string message) : base(message)
        {
        }

        public OppaiException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
