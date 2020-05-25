using Newtonsoft.Json;

namespace Chat.Dto.Input
{
    public class MemberDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("public_key")]
        public int PublicKey { get; private set; }
    }
}