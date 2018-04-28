using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace TAGGER.Commands
{
    internal class CommandHandler
    {
        private const int MaxTagValue = 16;

        [Command("nsfw")]
        public async Task Nsfw(CommandContext ctx, string argument = "")
        {
            var role = ctx.Guild.Roles.FirstOrDefault(x => x.Name == "NSFW");
            switch (argument)
            {
                case "add":
                    await Commands.Nsfw.Add(ctx, role);
                    break;
                case "remove":
                    await Commands.Nsfw.Remove(ctx, role);
                    break;
                default:
                    await ctx.RespondAsync($"{ctx.Member.Mention} please use either `!nsfw add` or `!nsfw remove` to recieve/remove the NSFW role.");
                    break;
            }
        }

        [Command("calculate")]
        public async Task Calculate(CommandContext ctx, string argument = "", int tag = 4)
        {
            if (argument == "")
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} please use `!calculate {{beatmap link}} [player count (default = 4, max = {MaxTagValue})]` to calculate the beatmap star rating in TAG mode.");
            }
            else if (tag < 1 || tag > MaxTagValue)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} player count must have a minimum of 1 and a maximum of {MaxTagValue}");
            }
            else
            {
                await Commands.Calculate.Sr(ctx, argument, tag, false);
            }
        }

        [Command("submit")]
        public async Task Submit(CommandContext ctx, string argument = "", string category = "", int tag = 4)
        {
            if (argument == "" || category == "")
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} please use `!submit {{beatmap link}} {{category (either 'unfinished' or 'complete')}} [player count (default = 4, max = {MaxTagValue})]` to submit the beatmap to the TAG map index.");
            }
            else if (tag < 1 || tag > MaxTagValue)
            {
                await ctx.RespondAsync(
                    $"{ctx.Member.Mention} player count must have a minimum of 1 and a maximum of {MaxTagValue}");
            }
            else
            {
                Commands.Calculate.ParseBeatmapIdFromUrl(argument, out var beatmapId, out var mode);
                var sr = await Commands.Calculate.Sr(ctx, argument, tag, true);

                if (sr == 0.0)
                    return;

                if (category != "complete" && category != "unfinished")
                    return;

                var url = $@"https://tag.tayo.ws/libs/add.php?apikey={Config.GetConfig("WebAPIKey")}&beatmap_id={beatmapId}&players={tag}&sr={sr}&category={category}&osukey={Config.GetConfig("OsuAPIKey")}";
                var request = (HttpWebRequest) WebRequest.Create(url);
                Console.WriteLine(url);
                var response = (HttpWebResponse) request.GetResponse();
                switch (response.StatusCode.ToString())
                {
                    case "200":
                        await ctx.RespondAsync($"{ctx.Member.Mention} beatmap was added to the database!");
                        break;
                    case "403":
                        await ctx.RespondAsync(
                            $"{ctx.Member.Mention} the API Key for `tag.tayo.ws` is invalid. Please report this to Tayo or open an issue on GitHub.");
                        break;
                    default:
                        await ctx.RespondAsync($"{ctx.Member.Mention} no response from the web service, please try again later. If the problem persists contact Tayo.");
                        break;
                }
            }
        }
    }
}
