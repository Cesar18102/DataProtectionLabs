using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using ChatServer.Dto.Output;

namespace ChatServer.Dto.Input
{
    public class CreateChatDto
    {
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("chat_name")]
        public string ChatName { get; private set; }

        [Required]
        [JsonProperty("creator")]
        public MemberDto Creator { get; private set; }

        [Required]
        [JsonProperty("p")]
        public int? P { get; private set; }

        [Required]
        [JsonProperty("g")]
        public int? G { get; private set; }
    }
}