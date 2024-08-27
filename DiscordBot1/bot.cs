using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot1;

public class Bot : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public Bot(IServiceProvider services, IConfiguration configuration)
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        _client = new DiscordSocketClient(config);
        _commands = new CommandService();
        _services = services;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;
        _commands.Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
        await _client.StartAsync();

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleCommandAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _client.LogoutAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return;

        Console.WriteLine($"Message: {message.Content ?? "null"}, Channel: {message.Channel.Name}, Author: {message.Author.Username}");
        
        int argPos = 0;
        if (!(message.HasCharPrefix('/', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        var context = new SocketCommandContext(_client, message);
        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error: {result.ErrorReason}");
        }
        else
        {
            Console.WriteLine($"Command '{message}' executed successfully.");
        }
    }
}