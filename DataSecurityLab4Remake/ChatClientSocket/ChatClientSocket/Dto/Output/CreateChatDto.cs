using ChatClientSocket.Dto.Input;

using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Output
{
    public class CreateChatDto
    {
        [JsonProperty("chat_name")]
        public string ChatName { get; set; }

        [JsonProperty("creator")]
        public MemberDto Creator { get; set; }

        [JsonProperty("p")]
        public int P { get; set; }

        [JsonProperty("g")]
        public int G { get; set; }
    }
}