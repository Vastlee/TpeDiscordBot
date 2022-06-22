using System;

namespace TpeDiscordBot;

[System.Serializable]
public struct BotSettings {
    public ulong GuildId { get; init; }
    public ulong WelcomeChannelId { get; init; }
    public ulong RolesMessageId { get; init; }
    public DateTime LastDYELTitleUpdate { get; init; }
}
