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
    public static async Task Give(CommandContext ctx, DiscordMember targetUser, uint amountToGive, DiscordEmoji emojiToGive) {
        StringBuilder message = new($"Congrats {targetUser.Mention}! {ctx.User.Mention} thinks you deserve ");
        Pluralize(ref message, amountToGive, emojiToGive);

        await MessageChannel(ctx.Channel, $"{message}");
    }

    [Command("NextBM")]
    public static async Task NextBM(CommandContext ctx, int currentDay) {
        StringBuilder message = new("The Next Blood Moon is ");

        int daysUntil = currentDay % 7;
        int nextBM = currentDay + 7 - daysUntil;

        if(daysUntil == 0) {
            message.Append("TODAY!!!");
        } else {
            message.Append($"on day {nextBM}!");
        }

        await MessageChannel(ctx.Channel, $"{message}");
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

        await MessageChannel(ctx.Channel, $"Today Is the {liftDay} of the routine. {flex}{flex}");
    }

    [Command("Praise")]
    public static async Task Priase(CommandContext ctx, DiscordMember targetUser) {
        foreach(DiscordMember user in ctx.Guild.Members.Values) {
            if(user.Id == targetUser.Id) {
                await MessageChannel(ctx.Channel, $"You're doing a great job {targetUser.DisplayName}!");
            }
        }
    }

    [Command("Sum")]
    [Description("Returns the Sum of the numbers")]
    public static async Task Sum(CommandContext ctx, params int[] nums) {
        int result = 0;
        for(int i = 0; i < nums.Length; i++) {
            result += nums[i];
        }
        await MessageChannel(ctx.Channel, $"{result}");
    }

    private static async Task MessageChannel(DiscordChannel channel, string message) {
        await channel.SendMessageAsync(message).ConfigureAwait(false);
    }

    private static void Pluralize(ref StringBuilder s, uint num, DiscordEmoji emoji) {
        if(num > 1) {
            s.Append($"{num} {emoji}'s");
        } else {
            s.Append($"a {emoji}");
        }
    }

    private static void Pluralize(ref StringBuilder s, int num) {
        if(num == 1) { s.Append("'s"); }
    }
}

