using Newtonsoft.Json;

namespace Chat.Dto.Input
{
    public class ChatInfoDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("p")]
        public int P { get; private set; }

        [JsonProperty("g")]
        public int G { get; private set; }
    }
}