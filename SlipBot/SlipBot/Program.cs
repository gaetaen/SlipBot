using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;
using Serilog;
using SlipBot.ButtonCommands;
using SlipBot.SlashCommands;

namespace SlipBot
{
    public static class Program
    {
        public static DiscordClient? Client { get; private set; }

        public static async Task Main(string[] args)
        {
            // Set up Serilog for logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var discordConfig = new DiscordConfiguration
                {
                    Intents = DiscordIntents.All,
                    Token = configuration.GetSection("DiscordSettings")["Token"],
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                };
                using var client = new DiscordClient(discordConfig);

                Client = client;
                RegisterEventHandlers();

                // Enable Interactivity module
                Client.UseInteractivity();

                var slashCommandsConfig = Client.UseSlashCommands();
                slashCommandsConfig.RegisterCommands<PollCommands>(639850180551901245);

                await Client.ConnectAsync();
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void RegisterEventHandlers()
        {
            if (Client == null)
            {
                throw new InvalidOperationException("Client is null. Cannot register event handlers.");
            }

            // Event handlers registration
            Client.Ready += Client_Ready;
            Client.ComponentInteractionCreated += ButtonResponse;
        }

        private static async Task ButtonResponse(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            try
            {
                if (args.Interaction.Data.CustomId == "endPoll")
                {
                    await ButtonInteractionHandler.EndVote(args);
                }
                else
                {
                    Log.Warning("Unexpected custom ID: {CustomId}", args.Interaction.Data.CustomId);
                    await args.Interaction.CreateResponseAsync(
                        InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Une erreur est survenue").AsEphemeral()
                    );
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ButtonResponse");

                await args.Interaction.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Une erreur est survenue").AsEphemeral()
                );
            }
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            // Initialization logic can be added here
            Log.Information("Bot is connected and ready!");
            return Task.CompletedTask;
        }
    }
}