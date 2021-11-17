using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TpeDiscordBot.Commands;

namespace TpeDiscordBot;

public class Bot {
    private enum RoleChangeAction { None, Add, Remove }

    public DiscordClient Client { get; private set; }
    public CommandsNextExtension Commands { get; private set; }
    public InteractivityExtension Interactivity { get; private set; }
    public BotSettings Settings { get; private set; }
    public DiscordMessage RolesMessage { get; private set; }

    private DiscordGuild TPEGuild { get; set; }
    private DiscordMessage TPERolesMessage { get; set; }
    private MemberRoleManager RoleManager { get; set; }

    public async Task RunAsync() {
        Settings = await GetBotSettings().ConfigureAwait(false);
        var botConfigJson = string.Empty;
        using(var fs = File.OpenRead("config.json")) {
            using(var sr = new StreamReader(fs, new UTF8Encoding(false))) {
                botConfigJson = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        ConfigJson configJson = JsonConvert.DeserializeObject<ConfigJson>(botConfigJson);

        var config = new DiscordConfiguration() {
            Token = configJson.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug,
        };

        Client = new DiscordClient(config);
        RoleManager = new MemberRoleManager(Client);
        Client.Ready += OnClientReady;

        TPEGuild = await Client.GetGuildAsync(Settings.GuildId).ConfigureAwait(false);

        Client.MessageReactionAdded += HandleRoleAddRequest;
        Client.MessageReactionRemoved += HandleRoleRemoveRequest;

        Client.UseInteractivity(new InteractivityConfiguration());

        var commandsConfig = new CommandsNextConfiguration() {
            StringPrefixes = new string[] { configJson.prefix },
            EnableDms = true,
            EnableMentionPrefix = true,
            DmHelp = true,

        };

        Commands = Client.UseCommandsNext(commandsConfig);
        Commands.RegisterCommands<FunCommands>();
        Commands.RegisterCommands<SystemCommands>();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    private async Task HandleRoleRemoveRequest(DiscordClient client, MessageReactionRemoveEventArgs reaction) {
        if(IsRoleReaction(reaction.Message.Id)) {
            var role = TPEGuild.GetRole(RoleManager.GetMemberRoleByEmoji(reaction.Emoji).RoleId);
            await reaction.Guild.Members[reaction.User.Id].RevokeRoleAsync(role).ConfigureAwait(false);
        }
    }

    private async Task HandleRoleAddRequest(DiscordClient client, MessageReactionAddEventArgs reaction) {
        if(IsRoleReaction(reaction.Message.Id)) {
            var role = TPEGuild.GetRole(RoleManager.GetMemberRoleByEmoji(reaction.Emoji).RoleId);
            await reaction.Guild.Members[reaction.User.Id].GrantRoleAsync(role).ConfigureAwait(false);
        }
    }

    private async Task<BotSettings> GetBotSettings() {
        string botSettingsJson = String.Empty;
        using(var fs = File.OpenRead("BotSettings.json")) {
            using(var sr = new StreamReader(fs, new UTF8Encoding(false))) {
                botSettingsJson = await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        return JsonConvert.DeserializeObject<BotSettings>(botSettingsJson);
    }

    private Task OnClientReady(DiscordClient client, ReadyEventArgs e) {
        return Task.CompletedTask;
    }

    private bool IsRoleReaction(ulong messageId) => (messageId == Settings.RolesMessageId);
}
