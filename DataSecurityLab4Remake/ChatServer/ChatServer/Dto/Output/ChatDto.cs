using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using ChatServer.Models;

namespace ChatServer.Dto.Output
{
    public class ChatDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("p")]
        public int P { get; private set; }

        [JsonProperty("g")]
        public int G { get; private set; }

        [JsonProperty("members")]
        public IEnumerable<MemberDto> Members { get; private set; }

        public ChatDto(Chat chat)
        {
            P = chat.P.Value;
            G = chat.G.Value;

            Name = chat.Name;
            Members = chat.Members.Keys.Select(member => new MemberDto(member));
        }
    }
}