using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace TAGGER.Commands
{
    internal static class Nsfw
    {
        public static async Task Add(CommandContext ctx, DiscordRole role)
        {
            if (!ctx.Member.Roles.Contains(role))
            {
                await ctx.Member.GrantRoleAsync(role);
                await ctx.RespondAsync($"{ctx.Member.Mention} you now have the NSFW role!");
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} you already have the NSFW role!");
            }
        }

        public static async Task Remove(CommandContext ctx, DiscordRole role)
        {
            if (ctx.Member.Roles.Contains(role))
            {
                await ctx.Member.RevokeRoleAsync(role);
                await ctx.RespondAsync($"{ctx.Member.Mention} you no longer have the NSFW role!");
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} you don't have the NSFW role!");
            }
        }
    }
}
