using Newtonsoft.Json;

using ChatServer.Models;

namespace ChatServer.Dto.Output
{
    public class MemberDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; private set; }

        [JsonProperty("port")]
        public int Port { get; private set; }

        [JsonConstructor]
        public MemberDto() { }

        public MemberDto(Member member)
        {
            Name = member.Name;
            Endpoint = member.Endpoint;
            Port = member.Port;
        }
    }
}