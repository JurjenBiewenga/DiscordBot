using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Services;

namespace DiscordBot.Commands
{
    [Name("RegisterInviteRoleLink")]
    public class RegisterInviteRoleLink : ModuleBase<SocketCommandContext>
    {
        private readonly StartupService startupService;
        private readonly CommandService commandService;

        public RegisterInviteRoleLink(StartupService service, CommandService commandService)
        {
            this.startupService = service;
            this.commandService = commandService;
        }

        [Command("Register")]
        [Alias("Reg")]
        [Summary("Registers an invite code to a specific role, Users joining through code will automatically get assigned the role.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Register(string code, SocketRole roleName)
        {
            await Register(code, roleName.Name);
        }

        [Command("Register")]
        [Alias("Reg")]
        [Summary("Registers an invite code to a specific role, Users joining through code will automatically get assigned the role.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Register(string code, string roleName)
        {
            var guild = Context.Guild;
            if (guild != null)
            {
                var invites = await guild.GetInvitesAsync();
                foreach (RestInviteMetadata restInviteMetadata in invites)
                {
                    if (restInviteMetadata.Code == code)
                    {
                        var links = Config.GetValue(new List<InviteRoleLink>(), "Data", "InviteRoleLinks");
                        SocketRole first = null;
                        foreach (SocketRole x in guild.Roles)
                        {
                            if (x.Name == roleName)
                            {
                                first = x;
                                break;
                            }
                        }

                        if (first == null)
                        {
                            await ReplyAsync("No valid role found");
                            return;
                        }
                        
                        var link = new InviteRoleLink(code, restInviteMetadata.Uses, first.Id);
                        links.Add(link);
                        Config.SetValue(links, "Data", "InviteRoleLinks");
                        await ReplyAsync("Succesfully registered");
                        return;
                    }
                }
                await ReplyAsync("No invite link with that code found");
            }
        }

        [Command("ListRoleLinks")]
        [Alias("LRL", "l")]
        [Summary("Lists all registered invite links")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task List()
        {
            var guild = Context.Guild;
            var links = Config.GetValue(new List<InviteRoleLink>(), "Data", "InviteRoleLinks");
            string output = "";
            foreach (InviteRoleLink inviteRoleLink in links)
            {
                output += inviteRoleLink.inviteCode + " : " + guild.GetRole(inviteRoleLink.roleId) + System.Environment.NewLine;
            }

            await ReplyAsync(output);
        }

        [Command("Remove")]
        [Alias("Rem")]
        [Summary("Removes a registered invite link")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Remove(string code)
        {
            var links = Config.GetValue(new List<InviteRoleLink>(), "Data", "InviteRoleLinks");
            links = links.Where(x => x.inviteCode != code).ToList();
            Config.SetValue(links, "Data", "InviteRoleLinks");
            await ReplyAsync("Succesfully removed " + code);
        }

        [Command("Help")]
        [Alias("h")]
        [Summary("Lists all available commands")]
        public async Task Help()
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