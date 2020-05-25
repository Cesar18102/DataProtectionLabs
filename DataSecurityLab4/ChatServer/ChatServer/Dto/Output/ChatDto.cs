using System.Collections.Generic;

using Newtonsoft.Json;

namespace ChatServer.Dto.Output
{
    public class ChatDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("members")]
        public IEnumerable<MemberDto> Members { get; private set; }

        [JsonProperty("messages")]
        public IEnumerable<MessageDto> Messages { get; private set; }

        public ChatDto(string name, IEnumerable<MemberDto> members, IEnumerable<MessageDto> messages)
        {
            Name = name;
            Members = members;
            Messages = messages;
        }
    }
}