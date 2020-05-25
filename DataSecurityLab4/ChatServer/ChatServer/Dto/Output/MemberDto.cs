using Newtonsoft.Json;

using ChatServer.Models;

namespace ChatServer.Dto.Output
{
    public class MemberDto
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("public_key")]
        public int PublicKey { get; private set; }

        public MemberDto(Member member)
        {
            Name = member.Name;
            PublicKey = member.PublicKey;
        }
    }
}