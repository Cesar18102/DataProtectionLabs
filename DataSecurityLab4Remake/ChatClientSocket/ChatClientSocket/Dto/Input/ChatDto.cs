using System.Collections.Generic;

using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Input
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
        public List<MemberDto> Members { get; private set; }
    }
}