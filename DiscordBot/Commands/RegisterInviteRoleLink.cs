using System;
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
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Register(string code, SocketRole roleName)
        {
            await Register(code, roleName.Name);
        }

        [Command("Register")]
        [Alias("Reg")]
        [Summary("Registers an invite code to a specific role, Users joining through code will automatically get assigned the role.")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Register(string code, string roleName)
        {
            var guild = Context.Guild;
            if (guild != null)
            {
                var invites = await guild.GetInvitesAsync();
                foreach (RestInviteMetadata restInviteMetadata in invites)
                {
                    if (String.Compare(restInviteMetadata.Code, code, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var links = Config.GetValue(new List<InviteRoleLink>(), "Data",  Context.Guild.Id.ToString(), "InviteRoleLinks");
                        SocketRole first = null;
                        foreach (SocketRole x in guild.Roles)
                        {
                            if (String.Compare(x.Name, roleName, StringComparison.OrdinalIgnoreCase) == 0)
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
                        
                        var link = new InviteRoleLink(roleName, code, restInviteMetadata.Uses, first.Id);
                        links.Add(link);
                        Config.SetValue(links, "Data", Context.Guild.Id.ToString(), "InviteRoleLinks");
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
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task List()
        {
            var guild = Context.Guild;
            var links = Config.GetValue(new List<InviteRoleLink>(), "Data",  Context.Guild.Id.ToString(), "InviteRoleLinks");
            links = links.OrderByDescending(x => guild.GetRole(x.roleId)).ToList();
            string output = "";
            foreach (InviteRoleLink inviteRoleLink in links)
            {
                output += inviteRoleLink.inviteCode + " : " + guild.GetRole(inviteRoleLink.roleId) + System.Environment.NewLine;
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                await ReplyAsync("No registered invite links found");
                return;
            }
            await ReplyAsync(output);
        }

        [Command("Remove")]
        [Alias("Rem")]
        [Summary("Removes a registered invite link")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Remove(string code)
        {
            var links = Config.GetValue(new List<InviteRoleLink>(), "Data",  Context.Guild.Id.ToString(), "InviteRoleLinks");
            links = links.Where(x => x.inviteCode != code).ToList();
            Config.SetValue(links, "Data",  Context.Guild.Id.ToString(), "InviteRoleLinks");
            await ReplyAsync("Succesfully removed " + code);
        }
        
        [Command("Convert")]
        [Alias("c")]
        [Summary("Converts invite links to the new format")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task ConvertToNewFormat()
        {
            var newLinks = Config.GetValue(new List<InviteRoleLink>(), "Data", Context.Guild.Id.ToString(), "InviteRoleLinks");
            foreach (InviteRoleLink inviteRoleLink in newLinks)
            {
                var role = Context.Guild.GetRole(inviteRoleLink.roleId);
                inviteRoleLink.roleName = role.Name;
            }
            Config.SetValue(newLinks, "Data", Context.Guild.Id.ToString(), "InviteRoleLinks");
            
            await ReplyAsync("Succesfully converted existing invite links");
        }
    }
}