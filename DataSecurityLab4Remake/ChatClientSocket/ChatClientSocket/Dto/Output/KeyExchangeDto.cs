using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Output
{
    public class KeyExchangeDto
    {
        [JsonProperty("key_temp")]
        public int KeyTemp { get; set; }

        [JsonProperty("ttl")]
        public int TTL { get; set; } = 1;
    }
}
