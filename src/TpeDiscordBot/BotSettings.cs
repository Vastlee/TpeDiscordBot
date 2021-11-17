using Newtonsoft.Json;

namespace TpeDiscordBot;

public struct BotSettings {
    [JsonProperty("GuildId")]
    public ulong GuildId { get; init; }
    [JsonProperty("WelcomeChannelId")]
    public ulong WelcomeChannelId { get; init; }
    [JsonProperty("RolesMessageId")]
    public ulong RolesMessageId { get; init; }
}
