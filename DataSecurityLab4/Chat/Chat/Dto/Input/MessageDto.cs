using System;

using Newtonsoft.Json;

namespace Chat.Dto.Input
{
    public class MessageDto
    {
        [JsonProperty("encoded_text")]
        public string EncodedText { get; private set; }

        [JsonProperty("sender")]
        public MemberDto Sender { get; private set; }

        [JsonProperty("time_span")]
        public DateTime TimeSpan { get; private set; }
    }
}