using Newtonsoft.Json;

namespace ChatClientSocket.Dto.Input.Exceptions
{
    public class NotFoundException : CustomException
    {
        [JsonRequired]
        [JsonProperty("not_found")]
        public string NotFoundSubject { get; private set; }

        public NotFoundException(string notFoundSubject)
        {
            NotFoundSubject = notFoundSubject;
        }

        public override string Message => $"{NotFoundSubject} not found";
    }
}