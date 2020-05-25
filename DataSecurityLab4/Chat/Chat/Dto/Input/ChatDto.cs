using System.Collections.Generic;

using Newtonsoft.Json;

namespace Chat.Dto.Input
{
    public class ChatDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("members")]
        public IEnumerable<MemberDto> Members { get; private set; }

        [JsonProperty("messages")]
        public IEnumerable<MessageDto> Messages { get; private set; }
    }
}