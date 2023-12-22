using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;

namespace SlipBot;

internal class Program
{
    private static DiscordClient Client { get; set; }

    public static async Task Main(string[] args)
    {
        // Build configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();

        var discordConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = configuration.GetSection("DiscordSettings")["Token"],
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        Client = new DiscordClient(discordConfig);
        Client.Ready += Client_Ready;

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }
}