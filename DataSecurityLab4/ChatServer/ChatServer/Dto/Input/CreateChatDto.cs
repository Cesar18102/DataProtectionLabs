using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace ChatServer.Dto.Input
{
    public class CreateChatDto
    {
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("name")]
        public string Name { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [JsonProperty("creator_name")]
        public string CreatorName { get; private set; }

        [Required]
        [JsonProperty("p")]
        public int? P { get; private set; }

        [Required]
        [JsonProperty("g")]
        public int? G { get; private set; }

        [Required]
        [JsonProperty("public_key")]
        public int? PublicKey { get; private set; }
    }
}