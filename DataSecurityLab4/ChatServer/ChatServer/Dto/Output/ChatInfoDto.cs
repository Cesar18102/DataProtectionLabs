using System;

using Newtonsoft.Json;

using ChatServer.Models;

namespace ChatServer.Dto.Output
{
    public class ChatInfoDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("p")]
        public int P { get; private set; }

        [JsonProperty("g")]
        public int G { get; private set; }

        public ChatInfoDto(Chat chat)
        {
            Name = chat.Name;

            if (!chat.P.HasValue || !chat.G.HasValue)
                throw new InvalidOperationException("chat keys must be set");

            P = chat.P.Value;
            G = chat.G.Value;
        }
    }
}