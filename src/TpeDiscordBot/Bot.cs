using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using TpeDiscordBot.Commands;
using System.Text.Json;

namespace TpeDiscordBot;

public class Bot {
    enum RoleChangeAction { None, Add, Remove }

    BotSettings settings;
    DiscordGuild tpeGuild;
    MemberRoleManager roleManager;
    
    public async Task RunAsync() {
        DiscordClient client;
        CommandsNextExtension commands;

        settings = await GetBotSettingsAsync().ConfigureAwait(false);
        string botConfigJson = string.Empty;
        using(FileStream fs = File.OpenRead("config.json")) {
            using var sr = new StreamReader(fs, new UTF8Encoding(false));
            botConfigJson = await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        ConfigJson configJson = JsonSerializer.Deserialize<ConfigJson>(botConfigJson);

        var config = new DiscordConfiguration() {
            Token = configJson.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug,
        };

        client = new DiscordClient(config);
        roleManager = new MemberRoleManager(client);
        client.Ready += OnClientReady;

        tpeGuild = await client.GetGuildAsync(settings.GuildId).ConfigureAwait(false);

        client.MessageReactionAdded += HandleRoleAddRequest;
        client.MessageReactionRemoved += HandleRoleRemoveRequest;

        client.UseInteractivity(new InteractivityConfiguration());

        CommandsNextConfiguration commandsConfig = new() {
            StringPrefixes = new string[] { configJson.Prefix },
            EnableDms = true,
            EnableMentionPrefix = true,
            DmHelp = true,
        };

        commands = client.UseCommandsNext(commandsConfig);
        commands.RegisterCommands<FunCommands>();

        await client.ConnectAsync();
        await Task.Delay(-1);
    }

    async Task HandleRoleRemoveRequest(DiscordClient client, MessageReactionRemoveEventArgs reaction) {
        if(IsRoleReaction(reaction.Message.Id)) {
            DiscordRole role = tpeGuild.GetRole(roleManager.GetMemberRoleByEmoji(reaction.Emoji).RoleId);
            await reaction.Guild.Members[reaction.User.Id].RevokeRoleAsync(role).ConfigureAwait(false);
        }
    }

    async Task HandleRoleAddRequest(DiscordClient client, MessageReactionAddEventArgs reaction) {
        if(IsRoleReaction(reaction.Message.Id)) {
            DiscordRole role = tpeGuild.GetRole(roleManager.GetMemberRoleByEmoji(reaction.Emoji).RoleId);
            await reaction.Guild.Members[reaction.User.Id].GrantRoleAsync(role).ConfigureAwait(false);
        }
    }

    static async Task<BotSettings> GetBotSettingsAsync() {
        string botSettingsJson = String.Empty;
        using(FileStream fs = File.OpenRead("BotSettings.json")) {
            using var sr = new StreamReader(fs, new UTF8Encoding(false));
            botSettingsJson = await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        return JsonSerializer.Deserialize<BotSettings>(botSettingsJson);
    }

    Task OnClientReady(DiscordClient client, ReadyEventArgs e) {
        new DYELService(client);
        return Task.CompletedTask;
    }

    bool IsRoleReaction(ulong messageId) => (messageId == settings.RolesMessageId);
}
