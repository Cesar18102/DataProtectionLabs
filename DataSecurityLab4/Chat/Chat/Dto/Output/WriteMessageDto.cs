using System.Collections.Generic;

using Newtonsoft.Json;

namespace Chat.Dto.Output
{
    public class WriteMessageDto
    {
        [JsonProperty("chat_name")]
        public string ChatName { get; private set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; private set; }

        [JsonProperty("message_table")]
        public IEnumerable<EncodedMessageDto> MessageTable { get; private set; }

        public WriteMessageDto(string chatName, string senderName, IEnumerable<EncodedMessageDto> messageTable)
        {
            ChatName = chatName;
            SenderName = senderName;
            MessageTable = messageTable;
        }
    }
}