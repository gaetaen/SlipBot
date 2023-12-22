using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SlipBot.SlashCommands
{
    public class FunSL : ApplicationCommandModule
    {
        [SlashCommand("Sondage", "Créer un vote")]
        public static async Task PollCommand(InteractionContext context,
            [Option("question", "La question du sondage")] string question,
            [Option("Option-1", "Option1")] string Option1,
            [Option("Option-2", "Option2")] string Option2,
            [Option("Option-3", "Option3")] string? Option3 = null,
            [Option("Option-4", "Option4")] string? Option4 = null,
            [Option("Option-5", "Option5")] string? Option5 = null)
        {
            DiscordEmoji[] optionEmojiArray = {
                DiscordEmoji.FromName(Program.Client, ":one:", false),
                DiscordEmoji.FromName(Program.Client, ":two:", false),
                DiscordEmoji.FromName(Program.Client, ":three:", false),
                DiscordEmoji.FromName(Program.Client, ":four:", false),
                DiscordEmoji.FromName(Program.Client, ":five:", false)
            };

            List<string?> optionList = new List<string?> { Option1, Option2, Option3, Option4, Option5 };
            List<string> optionArray = optionList.Where(option => option != null).Select(option => option!).ToList();

            var embedContent = new StringBuilder();
            embedContent.AppendLine($"**{question}**\n");

            for (int i = 0; i < optionArray.Count; i++)
            {
                embedContent.AppendLine($"{optionEmojiArray[i]} | **{optionArray[i]}**");
                embedContent.AppendLine();
            }

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Sondage")
                .WithDescription(embedContent.ToString())
                .WithColor(DiscordColor.Azure)
                .Build();

            var message = await context.Channel.SendMessageAsync(embed: embed);

            for (int i = 0; i < optionArray.Count; i++)
            {
                await message.CreateReactionAsync(optionEmojiArray[i]);
            }
        }
    }
}