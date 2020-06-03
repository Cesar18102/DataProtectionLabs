using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Input.Exceptions
{
    public class ConflictException : CustomException
    {
        [JsonRequired]
        [JsonProperty("conflict")]
        public string ConflictSubject { get; private set; }

        public ConflictException(string conflictSubject)
        {
            ConflictSubject = conflictSubject;
        }

        public override string Message => $"A conflict on {ConflictSubject} occured";
    }
}