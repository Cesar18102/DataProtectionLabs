using Newtonsoft.Json;

namespace Chat.Dto.Output
{
    public class JoinChatDto
    {
        [JsonProperty("chat_name")]
        public string ChatName { get; private set; }

        [JsonProperty("member_name")]
        public string MemberName { get; private set; }

        [JsonProperty("public_key")]
        public int PublicKey { get; private set; }

        public JoinChatDto(string chatName, string memberName, int publicKey)
        {
            ChatName = chatName;
            MemberName = memberName;
            PublicKey = publicKey;
        }
    }
}