using Newtonsoft.Json;

namespace ChatServer.Dto.Output.Exceptions
{
    public class ConflictException : CustomException
    {
        [JsonProperty("conflict")]
        public string ConflictSubject { get; private set; }

        public ConflictException(string conflictSubject)
        {
            ConflictSubject = conflictSubject;
        }

        public override string Message => $"A conflict on {ConflictSubject} occured";
    }
}