using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Commands
{
    public class Help :  ModuleBase<SocketCommandContext>
    {
        
        private readonly CommandService commandService;

        public Help(CommandService commandService)
        {
            this.commandService = commandService;
        }

        [Command("Help")]
        [Alias("h")]
        [Summary("Lists all available commands")]
        public async Task HelpAsync()
        {
            string helpText = "Available commands: " + System.Environment.NewLine;
            var distinct = commandService.Commands.GroupBy(x => x.Name).Select(x => x.First());
            foreach (CommandInfo commandServiceCommand in distinct)
            {
                helpText += commandServiceCommand.Name + " : " + commandServiceCommand.Summary + System.Environment.NewLine;
            }
            await ReplyAsync(helpText);
        }
    }
}