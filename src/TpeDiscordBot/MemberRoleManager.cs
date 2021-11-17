using System.Collections.Generic;
using DSharpPlus;
using DSharpPlus.Entities;

namespace TpeDiscordBot {
    public class MemberRoleManager {
        private const ulong CODER_ROLE_ID = 905814098724392980;
        private const ulong DESIGNER_ROLE_ID = 905815273448288257;
        private const ulong ARTIST_ROLE_ID = 905815893546795030;
        private const ulong AUDIO_ROLE_ID = 905816151001559040;

        public List<MemberRole> Roles { get; private set; } = new();
        public MemberRole Coder { get; private set; }
        public MemberRole Designer { get; private set; }
        public MemberRole Artist { get; private set; }
        public MemberRole Audio { get; private set; }


        public MemberRoleManager(DiscordClient client) {
            Coder = new MemberRole() {
                Type = MemberRole.RoleType.Coder,
                RoleId = CODER_ROLE_ID,
                Emoji = DiscordEmoji.FromName(client, ":nerd:"),
            };

            Designer = new MemberRole() {
                Type = MemberRole.RoleType.Designer,
                RoleId = DESIGNER_ROLE_ID,
                Emoji = DiscordEmoji.FromName(client, ":pencil:")
            };

            Artist = new MemberRole() {
                Type = MemberRole.RoleType.Artist,
                RoleId = ARTIST_ROLE_ID,
                Emoji = DiscordEmoji.FromName(client, ":paintbrush:")
            };

            Audio = new MemberRole() {
                Type = MemberRole.RoleType.Musician,
                RoleId = AUDIO_ROLE_ID,
                Emoji = DiscordEmoji.FromName(client, ":headphones:")
            };

            Roles.Add(Coder);
            Roles.Add(Designer);
            Roles.Add(Artist);
            Roles.Add(Audio);
        }

        public MemberRole GetMemberRoleByEmoji(DiscordEmoji emoji) {
            foreach(MemberRole role in Roles) {
                if(role.Emoji == emoji) { return role; }
            }
            return null;
        }
    }
}
