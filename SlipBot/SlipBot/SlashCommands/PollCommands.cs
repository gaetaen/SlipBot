using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Serilog;
using System.Text;

namespace SlipBot.SlashCommands
{
    public class PollCommands : ApplicationCommandModule
    {
        [SlashCommand("Poll", "Créer un sondage")]
        public static async Task CreatePoll(
            InteractionContext context,
            [Option("question", "La question du sondage")] string question,
            [Option("Option-1", "Option1")] string Option1,
            [Option("Option-2", "Option2")] string Option2,
            [Option("Option-3", "Option3")] string? Option3 = null,
            [Option("Option-4", "Option4")] string? Option4 = null,
            [Option("Option-5", "Option5")] string? Option5 = null)
        {
            try
            {
                // Defer de response to user
                await context.DeferAsync(true);

                // Map options to a dictionary for easier handling
                var options = new Dictionary<string, string?>
                {
                    ["Option-1"] = Option1,
                    ["Option-2"] = Option2,
                    ["Option-3"] = Option3,
                    ["Option-4"] = Option4,
                    ["Option-5"] = Option5
                };

                // Filter out null options and limit the number of options
                var validOptions = options.Values.Where(option => option != null).Take(5).ToList();

                if (validOptions.Count < 2)
                {
                    // Handle the case where there are not enough valid options
                    await context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Le sondage doit avoir au moins deux options.").AsEphemeral());
                    return;
                }

                // Create a list of option tuples with emojis
                var optionEmojiArray = validOptions
                    .Select((option, index) => (Emoji: DiscordEmoji.FromName(Program.Client, $"{NumberToEmoji(index + 1)}", false), Option: option))
                    .ToList();

                // Build the embed content
                var embedContent = new StringBuilder();
                foreach (var (emoji, option) in optionEmojiArray)
                {
                    embedContent.AppendLine($"{emoji} | **{option}** \n");
                }

                // Build and send the poll message with reactions
                const string pollIcon = "https://cdn.discordapp.com/attachments/695247614089887815/1189722745026662581/iu-removebg-preview.png?ex=659f32a6&is=658cbda6&hm=31e8cf15b9dcca5744beb0cff9c9a12c4b694f941ec55280853b8e51897f84a5&";
                var embed = new DiscordEmbedBuilder()
                    .WithDescription(embedContent.ToString())
                    .WithColor(DiscordColor.Azure)
                    .WithThumbnail(pollIcon)
                    .WithTitle($"**{question}**")
                    .WithAuthor(context.User.Username, iconUrl: context.User.AvatarUrl)
                    .Build();

                var message = new DiscordMessageBuilder().AddEmbed(embed);
                var sendedMessage = await context.Channel.SendMessageAsync(message);

                // Add reactions to the message
                foreach (var (Emoji, Option) in optionEmojiArray)
                {
                    await Task.Delay(300);
                    await sendedMessage.CreateReactionAsync(Emoji);
                }

                await context.FollowUpAsync(
                    new DiscordFollowupMessageBuilder().WithContent("Sondage créé avec succès").AsEphemeral()
                );
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Log.Error(ex, "An error occurred while creating a poll");

                // Inform the user about the error
                await context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Une erreur s'est produite lors de la création du sondage.").AsEphemeral());
            }
        }

        // Helper method to convert numbers to textual emojis
        private static string NumberToEmoji(int number)
        {
            switch (number)
            {
                case 1: return ":one:";
                case 2: return ":two:";
                case 3: return ":three:";
                case 4: return ":four:";
                case 5: return ":five:";
                default: return "";
            }
        }
    }
}