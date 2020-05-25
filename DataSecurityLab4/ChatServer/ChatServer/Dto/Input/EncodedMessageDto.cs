using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace ChatServer.Dto.Input
{
    public class EncodedMessageDto
    {
        [Required]
        [JsonProperty("encoded_text")]
        public string EncodedText { get; private set; }

        [Required]
        [JsonProperty("reciever_name")]
        public string RecieverName { get; private set; }
    }
}