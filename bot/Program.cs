using System.IO;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using TAGGER.Commands;
using TAGGER.Utils;

namespace TAGGER
{
    public class Bot
    {
        public static void Main(string[] args)
        {
            if (!Config.Exists())
                Config.Make();

            if (Config.GetConfig("DiscordToken") != "")
            {
                var config = new DiscordConfiguration
                {
                    Token = Config.GetConfig("DiscordToken"),
                    TokenType = TokenType.Bot,
                    UseInternalLogHandler = true,
                    LogLevel = LogLevel.Debug
                };
                Run(config).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            else
            {
                Logger.Warning("Please set your Discord Token in config.txt");
            }
        }

        private static async Task Run(DiscordConfiguration config)
        {
            var discord = new DiscordClient(config);
            await discord.ConnectAsync();

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            commands.RegisterCommands<CommandHandler>();

            await Task.Delay(-1);
        }
    }
}
