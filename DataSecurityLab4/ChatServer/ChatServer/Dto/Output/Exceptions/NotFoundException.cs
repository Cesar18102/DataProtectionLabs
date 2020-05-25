using Newtonsoft.Json;

namespace ChatServer.Dto.Output.Exceptions
{
    public class NotFoundException : CustomException
    {
        [JsonProperty("not_found")]
        public string NotFoundSubject { get; private set; }

        public NotFoundException(string notFoundSubject)
        {
            NotFoundSubject = notFoundSubject;
        }

        public override string Message => $"{NotFoundSubject} not found";
    }
}