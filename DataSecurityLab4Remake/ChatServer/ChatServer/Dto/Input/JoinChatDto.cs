using System.ComponentModel.DataAnnotations;
using ChatServer.Dto.Output;
using Newtonsoft.Json;

namespace ChatServer.Dto.Input
{
    public class JoinChatDto
    {
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("chat_name")]
        public string ChatName { get; set; }

        [Required]
        [JsonProperty("member")]
        public MemberDto Member { get; set; }
    }
}