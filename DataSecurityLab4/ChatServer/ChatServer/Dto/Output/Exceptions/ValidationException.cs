using Newtonsoft.Json;

namespace ChatServer.Dto.Output.Exceptions
{
    public class ValidationException : CustomException
    {
        [JsonProperty("invalid")]
        public string ValidationFailedSubject { get; private set; }

        public ValidationException(string validationFailedSubject)
        {
            ValidationFailedSubject = validationFailedSubject;
        }

        public override string Message => $"Validation failed on {ValidationFailedSubject}";
    }
}