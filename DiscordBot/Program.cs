using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public List<InviteRoleLink> inviteLinks = new List<InviteRoleLink>();

        public async Task MainAsync()
        {

            var services = new ServiceCollection() // Begin building the service provider
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig // Add the discord client to the service provider
                                                      {
                                                          LogLevel = LogSeverity.Verbose,
                                                          MessageCacheSize = 1000 // Tell Discord.Net to cache 1000 messages per channel
                                                      }))
                .AddSingleton(new CommandService(new CommandServiceConfig // Add the command service to the service provider
                                                 {
                                                     DefaultRunMode = RunMode.Async, // Force all commands to run async
                                                     LogLevel = LogSeverity.Verbose
                                                 }))
                .AddSingleton<CommandHandler>() 
                .AddSingleton<StartupService>()
                .AddSingleton<Random>();
            
            var provider = services.BuildServiceProvider();     // Create the service provider
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();
            
            await Task.Delay(-1);
        }
    }
}