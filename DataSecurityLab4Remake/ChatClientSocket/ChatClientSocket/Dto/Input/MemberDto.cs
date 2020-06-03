using System;

using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Input
{
    public class MemberDto : IEquatable<MemberDto>
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        public bool Equals(MemberDto other)
        {
            return Name == other.Name;
        }
    }
}