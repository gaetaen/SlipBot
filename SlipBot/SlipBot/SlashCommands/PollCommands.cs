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
using static System.Net.WebRequestMethods;

namespace SlipBot.SlashCommands
{
    public class PollCommands : ApplicationCommandModule
    {
        [SlashCommand("Créer_sondage", "Créer un sondage")]
        public static async Task CreatePoll(InteractionContext context,
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

            List<string?> optionList = new() { Option1, Option2, Option3, Option4, Option5 };
            List<string> optionArray = optionList.Where(option => option != null).Select(option => option!).ToList();

            var embedContent = new StringBuilder();
            embedContent.AppendLine($"**{question}** \n");

            for (int i = 0; i < optionArray.Count; i++)
            {
                embedContent.AppendLine($"﻿{optionEmojiArray[i]} | **{optionArray[i]}** \n");
            }

            var pollIcon = "https://cdn.discordapp.com/attachments/695247614089887815/1189722745026662581/iu-removebg-preview.png?ex=659f32a6&is=658cbda6&hm=31e8cf15b9dcca5744beb0cff9c9a12c4b694f941ec55280853b8e51897f84a5&";

            var embed = new DiscordEmbedBuilder()
                .WithDescription(embedContent.ToString())
                .WithColor(DiscordColor.Azure)
                .WithThumbnail(pollIcon)
                .Build();

            var message = new DiscordMessageBuilder()
                .AddEmbed(embed);

            var sendedMessage = await context.Channel.SendMessageAsync(message);

            for (int i = 0; i < optionArray.Count; i++)
            {
                await sendedMessage.CreateReactionAsync(optionEmojiArray[i]);
            }
        }
    }
}