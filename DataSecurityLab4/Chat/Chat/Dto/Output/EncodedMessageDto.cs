using Newtonsoft.Json;

namespace Chat.Dto.Output
{
    public class EncodedMessageDto
    {
        [JsonProperty("encoded_text")]
        public string EncodedText { get; private set; }

        [JsonProperty("reciever_name")]
        public string RecieverName { get; private set; }

        public EncodedMessageDto(string encodedText, string recieverName)
        {
            EncodedText = encodedText;
            RecieverName = recieverName;
        }
    }
}