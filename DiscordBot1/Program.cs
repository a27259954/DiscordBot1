using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot1;

var discordToken = Environment.GetEnvironmentVariable("DiscordToken");
if (string.IsNullOrEmpty(discordToken))
{
    throw new InvalidOperationException("DiscordToken environment variable is not set.");
}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<Bot>();
        services.AddHostedService<Bot>();
        services.AddSingleton<CommandService>();
        services.AddSingleton(new DiscordSocketClient());
    })
    .Build();

var client = host.Services.GetRequiredService<DiscordSocketClient>();
await client.LoginAsync(TokenType.Bot, discordToken);
await client.StartAsync();

await host.RunAsync();