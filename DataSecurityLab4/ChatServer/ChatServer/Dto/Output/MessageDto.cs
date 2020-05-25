using System;

using Newtonsoft.Json;

namespace ChatServer.Dto.Output
{
    public class MessageDto
    {
        [JsonProperty("encoded_text")]
        public string EncodedText { get; private set; }

        [JsonProperty("sender")]
        public MemberDto Sender { get; private set; }

        [JsonProperty("time_span")]
        public DateTime TimeSpan { get; private set; }

        public MessageDto(string text, MemberDto sender, DateTime timeSpan)
        {
            Sender = sender;
            EncodedText = text;
            TimeSpan = timeSpan;
        }
    }
}