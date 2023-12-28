using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;
using SlipBot.ButtonCommands;
using SlipBot.SlashCommands;
namespace SlipBot;

public sealed class Program
{
    private Program()
    { }

    public static DiscordClient? Client { get; set; }

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
        Client.ComponentInteractionCreated += ButtonResponse;

        var slashCommandsConfig = Client.UseSlashCommands();
        slashCommandsConfig.RegisterCommands<PollCommands>(639850180551901245);

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    private static async Task ButtonResponse(DiscordClient sender, ComponentInteractionCreateEventArgs args)
    {
        if (args.Interaction.Data.CustomId == "1")
        {
            await ButtonInteractionHandler.Vote(args);
        }
        else
        {
            await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("Une erreur s'est produite"));
        }
    }

    private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }
}