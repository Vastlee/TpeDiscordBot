using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace TpeDiscordBot.Commands;

internal class FunCommands : BaseCommandModule {
    private readonly DateTime DYELStartTime = new(2021, 11, 3);

    [Command("Give")]
    public async Task Give(CommandContext ctx, DiscordMember targetUser, uint amountToGive, DiscordEmoji emojiToGive) {
        StringBuilder message = new($"Congrats {targetUser.Mention}! {ctx.User.Mention} thinks you deserve ");
        Pluralize(ref message, amountToGive, emojiToGive);

        await ctx.Channel.SendMessageAsync($"{message}!").ConfigureAwait(false);
    }

    [Command("DYEL")]
    public async Task DYEL(CommandContext ctx) {
        var flex = DiscordEmoji.FromName(ctx.Client, ":muscle:");
        int sinceStart = DateTime.Now.Subtract(DYELStartTime).Days % 6;
        string liftDay = sinceStart switch {
            0 => "First Push Day!",
            1 => "First Pull Day!",
            2 => "First Hybrid Day!",
            3 => "Second Push Day!",
            4 => "Second Pull Day!",
            5 => "Second Hybrid Day!",
            _ => "... something broke. I have no idea what day it is!"
        };

        await ctx
            .Channel
            .SendMessageAsync($"Today Is the {liftDay} of the routine. {flex}{flex}{flex}")
            .ConfigureAwait(false);
    }

    [Command("Praise")]
    public async Task Priase(CommandContext ctx, DiscordMember targetUser) {
        foreach(DiscordMember user in ctx.Guild.Members.Values) {
            if(user.Id == targetUser.Id) {
                await ctx
                    .Channel
                    .SendMessageAsync($"You're doing a great job {targetUser.DisplayName}!")
                    .ConfigureAwait(false);
            }
        }
    }

    [Command("Sum")]
    [Description("Returns the Sum of the numbers")]
    public async Task Sum(CommandContext ctx, params int[] nums) {
        int result = 0;
        for(int i = 0; i < nums.Length; i++) {
            result += nums[i];
        }
        await ctx
            .Channel
            .SendMessageAsync($"{result}")
            .ConfigureAwait(false);
    }

    private void Pluralize(ref StringBuilder s, uint num, DiscordEmoji emoji) {
        if(num > 1) {
            s.Append($"{num} {emoji}'s");
        } else {
            s.Append($"a {emoji}");
        }
    }
}

