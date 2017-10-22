using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Commands
{
    public class GuildId :  ModuleBase<SocketCommandContext>
    {
        [Command("GetGuildID")]
        [Alias("ggid")]
        [Summary("Lists the guild id")]
        public async Task GetGuildId()
        {
            await ReplyAsync(Context.Guild.Id.ToString());
        }
    }
}