using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace ChatServer.Dto.Input
{
    public class WriteMessageDto
    {
        [Required]
        [JsonProperty("chat_name")]
        public string ChatName { get; private set; }

        [Required]
        [JsonProperty("sender_name")]
        public string SenderName { get; private set; }

        [Required]
        [JsonProperty("message_table")]
        public IEnumerable<EncodedMessageDto> MessageTable { get; private set; }
    }
}