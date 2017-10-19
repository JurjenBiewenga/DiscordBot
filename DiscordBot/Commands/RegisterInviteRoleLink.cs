using System.Collections.Generic;
using System.Linq;
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
        private readonly StartupService service;

        public RegisterInviteRoleLink(StartupService service)
        {
            this.service = service;
        }
        
        [Command("Register")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Register(string code, string roleName)
        {
            var guild = service.discord.Guilds.FirstOrDefault();
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
    }
}