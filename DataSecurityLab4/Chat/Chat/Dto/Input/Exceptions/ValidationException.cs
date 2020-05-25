using Newtonsoft.Json;

namespace Chat.Dto.Input.Exceptions
{
    public class ValidationException : CustomException
    {
        [JsonRequired]
        [JsonProperty("invalid")]
        public string ValidationFailedSubject { get; private set; }

        public ValidationException(string validationFailedSubject)
        {
            ValidationFailedSubject = validationFailedSubject;
        }

        public override string Message => $"Validation failed on {ValidationFailedSubject}";
    }
}