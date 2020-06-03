using System;

using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Input.Exceptions
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CustomException : Exception
    {
        [JsonProperty("message")]
        public override string Message => base.Message;
    }
}