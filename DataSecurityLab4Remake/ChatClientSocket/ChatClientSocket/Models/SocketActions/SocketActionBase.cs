using Newtonsoft.Json;

namespace ChatClientSocket.Models.SocketActions
{
    public class SocketActionBase
    {
        [JsonProperty("action")]
        public SocketActions Action { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
