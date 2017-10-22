using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Services;

namespace DiscordBot.Commands
{
    public class Prefix : ModuleBase<SocketCommandContext>
    {
        [Command("SetPrefix")]
        [Alias("sp")]
        [Summary("Sets the prefix")]
        public async Task SetPrefix(string prefix)
        {
            Config.SetValue(prefix, "Data", "Prefix");
            await ReplyAsync("Prefix succesfully set");
        }
    }
}