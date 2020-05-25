using Newtonsoft.Json;

namespace Chat.Dto.Output
{
    public class CreateChatDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("creator_name")]
        public string CreatorName { get; private set; }

        [JsonProperty("public_key")]
        public int PublicKey { get; private set; }

        [JsonProperty("p")]
        public int P { get; private set; }

        [JsonProperty("g")]
        public int G { get; private set; }

        public CreateChatDto(string name, string creatorName, int p, int g, int publicKey)
        {
            Name = name;
            CreatorName = creatorName;
            PublicKey = publicKey;
            P = p;
            G = g;
        }
    }
}