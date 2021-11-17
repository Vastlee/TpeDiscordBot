using Newtonsoft.Json;

namespace TpeDiscordBot;

public struct ConfigJson {
    [JsonProperty("token")]
    public string Token { get; private set; }
    [JsonProperty("prefix")]
    public string prefix { get; private set; }
}
