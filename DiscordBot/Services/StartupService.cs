using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot.Services
{
    public class StartupService
    {
        public readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            DiscordSocketClient discord,
            CommandService commands)
        {
            this.discord = discord;
            this.commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = Config.GetValue("", "Account", "Token");
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Config.SetValue("Insert token here", "Account", "Token");
                throw new Exception("Please enter your bot's token into the `Config.json` file found in the applications root directory.");
            }

            discord.Log += Log;
            
            await discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await discord.StartAsync();                                // Connect to the websocket

            discord.UserJoined += OnUserJoined;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());     // Load commands and modules into the command service
        }

        private async Task OnUserJoined(SocketGuildUser socketGuildUser)
        {
            var links = Config.GetValue(new List<InviteRoleLink>(), "Data", "InviteRoleLinks");
            foreach (InviteRoleLink inviteLink in links)
            {
                var guild = socketGuildUser.Guild;
                var invites = await guild.GetInvitesAsync();
                foreach (RestInviteMetadata restInviteMetadata in invites)
                {
                    if (inviteLink.inviteUsageCount + 1 == restInviteMetadata.Uses && inviteLink.inviteCode == restInviteMetadata.Code)
                    {
                        await socketGuildUser.AddRoleAsync(guild.GetRole(inviteLink.roleId));
                        inviteLink.inviteUsageCount = restInviteMetadata.Uses;
                        Config.SetValue(links, "Data", "InviteRoleLinks");
                        return;
                    }
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}