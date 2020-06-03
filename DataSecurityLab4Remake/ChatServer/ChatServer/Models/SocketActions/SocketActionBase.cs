using Newtonsoft.Json;

namespace ChatServer.Models.SocketActions
{
    public abstract class SocketActionBase
    {
        [JsonProperty("action")]
        public string Action { get; protected set; }

        [JsonProperty("data")]
        public object Data { get; protected set; }
    }
}