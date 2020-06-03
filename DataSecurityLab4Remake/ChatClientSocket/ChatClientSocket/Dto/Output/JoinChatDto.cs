using ChatClientSocket.Dto.Input;

using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Output
{
    public class JoinChatDto
    {
        [JsonProperty("chat_name")]
        public string ChatName { get; set; }

        [JsonProperty("member")]
        public MemberDto Member { get; set; }
    }
}