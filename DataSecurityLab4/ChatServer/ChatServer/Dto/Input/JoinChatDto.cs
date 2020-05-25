using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Dto.Input
{
    public class JoinChatDto
    {
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("chat_name")]
        public string ChatName { get; private set; }

        [Required(AllowEmptyStrings = false)]
        [JsonProperty("member_name")]
        public string MemberName { get; private set; }

        [Required]
        [JsonProperty("public_key")]
        public int? PublicKey { get; private set; }
    }
}