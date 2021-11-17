using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using Emzi0767.Utilities;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace TpeDiscordBot.Commands;

internal class SystemCommands : BaseCommandModule {
    [Command("Clear"), RequireRoles(RoleCheckMode.Any, "Admin")]
    public async Task Clear(CommandContext ctx) {
        var messages = await ctx.Channel.GetMessagesAsync(short.MaxValue).ConfigureAwait(false);
        int pause = ctx.Channel.PerUserRateLimit ?? 0;
        foreach(var message in messages) {
            await message.DeleteAsync().ConfigureAwait(false);
        }
    }

    [Command("Roles"), RequireRoles(RoleCheckMode.Any, "Admin")]
    public async Task Roles(CommandContext ctx) {
        MemberRoleManager roleManager = new MemberRoleManager(ctx.Client);
        StringBuilder embedMessage = new("Which Role are you MOST comfortable in?\n");

        EmbedThumbnail avatarThumbnail = new() {
            Url = ctx.Client.CurrentUser.AvatarUrl
        };

        foreach(MemberRole role in roleManager.Roles) {
            embedMessage.Append($"{role.Emoji}\t:\t{ctx.Guild.GetRole(role.RoleId).Name}\n");
        }

        DiscordEmbedBuilder joinEmbed = new() {
            Title = embedMessage.ToString(),
            Thumbnail = avatarThumbnail,
            Color = DiscordColor.CornflowerBlue,
        };

        var interactivity = ctx.Client.GetInteractivity();
        var joinMessage = await ctx.Channel.SendMessageAsync(joinEmbed).ConfigureAwait(false);

        await ctx.Message.DeleteAsync().ConfigureAwait(false);
        await joinMessage.CreateReactionAsync(roleManager.Coder.Emoji).ConfigureAwait(false);
        await joinMessage.CreateReactionAsync(roleManager.Designer.Emoji).ConfigureAwait(false);
        await joinMessage.CreateReactionAsync(roleManager.Artist.Emoji).ConfigureAwait(false);
        await joinMessage.CreateReactionAsync(roleManager.Audio.Emoji).ConfigureAwait(false);
    }
}

