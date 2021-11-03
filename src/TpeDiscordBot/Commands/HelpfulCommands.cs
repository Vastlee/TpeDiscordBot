using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Humanizer;

namespace TPEBot.Commands {
    public class HelpfulCommands : BaseCommandModule {
        private readonly DateTime DYELStartTime = new(2021, 11, 3);

        //[Command("Response")]
        //public async Task Response(CommandContext ctx) {
        //    var interactivity = ctx.Client.GetInteractivity();
        //    var message = await interactivity
        //        .WaitForMessageAsync(x => x.Channel == ctx.Channel)
        //        .ConfigureAwait(false);

        //    await ctx.Channel.SendMessageAsync(message.Result.Content);
        //}

        [Command("Give")]
        public async Task Give(CommandContext ctx, DiscordMember targetUser, int amountToGive, DiscordEmoji emojiToGive) {
            StringBuilder message = new($"Congrats {targetUser.Mention}! {ctx.User.Mention} thinks you deserve ");
            if(amountToGive > 1) {
                message.Append($"{amountToGive} {emojiToGive}{SIfPlural(amountToGive)}");
            } else {
                message.Append($"a {emojiToGive}");
            }

            await ctx
                .Channel
                .SendMessageAsync($"{message}!")
                .ConfigureAwait(false);
        }

        [Command("DYEL")]
        public async Task DYEL(CommandContext ctx) {
            var flex = DiscordEmoji.FromName(ctx.Client, ":muscle:");
            int sinceStart = DateTime.Now.Subtract(DYELStartTime).Days % 3;
            string liftDay = sinceStart switch {
                0 => "Push Day!",
                1 => "Pull Day!",
                2 => "Hybrid Day!",
                _ => "... something broke. I have no idea what day it is!"
            };

            await ctx
                .Channel
                .SendMessageAsync($"Today Is {liftDay} {flex}{flex}{flex}")
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

        private string SIfPlural(int num) => (num > 1) ? "'s" : string.Empty;
    }
}
