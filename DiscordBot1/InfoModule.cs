using Discord.Commands;

namespace DiscordBot1;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("hello")]
    public async Task HelloAsync()
    {
        await ReplyAsync("Hello, world!");
    }
    
    [Command("testargs")]
    public async Task TestArgsAsync(string arg)
    {
        await ReplyAsync($"Received argument: {arg}");
    }
}