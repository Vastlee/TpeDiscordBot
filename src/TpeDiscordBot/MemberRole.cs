using DSharpPlus.Entities;

namespace TpeDiscordBot {
    public class MemberRole {
        public enum RoleType {
            None,
            Protege,
            Coder,
            Designer,
            Artist,
            Musician
        }

        public RoleType Type { get; init; } = RoleType.None;
        public ulong RoleId { get; init; }
        public DiscordEmoji Emoji { get; init; }
    }
}
