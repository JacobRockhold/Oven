using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Oven.Bot.Services;

namespace Oven.Bot
{
    public class OvenDiscordClient
    {
        private DiscordSocketClient _client;

        public async Task LaunchAsync()
        {
            _client = new DiscordSocketClient();
            var services = ConfigureServices();
            services.GetRequiredService<LoggingService>(); //this is needed to initialise the logger. There's probably a nicer way to do this, but prototype code, so whatever.
            await services.GetRequiredService<CommandHandlingService>().InitialiseAsync(services).ConfigureAwait(false);
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN")).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<LoggingService>()
                .AddHttpClient()
                .AddTransient<IVodConfigurationService, JsonVodConfigurationService>()
                .BuildServiceProvider();
        }
    }
}