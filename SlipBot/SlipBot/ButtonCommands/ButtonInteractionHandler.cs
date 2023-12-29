using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Interactivity.Extensions;

namespace SlipBot.ButtonCommands
{
    public static class ButtonInteractionHandler
    {
        public static async Task EndVote(ComponentInteractionCreateEventArgs args)
        {
            await args.Message.DeleteAllReactionsAsync();
            await args.Interaction.CreateResponseAsync(
                InteractionResponseType.UpdateMessage,
                new DiscordInteractionResponseBuilder());
        }
    }
}