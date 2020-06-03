using System;

using Newtonsoft.Json;

namespace ChatClientSocket.Models
{
    public class Message
    {
        [JsonProperty("time_stamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("sender_name")]
        public string SenderName { get; set; }

        [JsonProperty("message")]
        public string MessageText { get; set; }
    }
}
