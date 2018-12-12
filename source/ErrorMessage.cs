using System.Collections.Immutable;
using System.Text;

namespace HandlingOperationResult
{
    public static class ErrorMessage
    {
        private static ImmutableDictionary<ErrorCode, string> errorMessages;

        static ErrorMessage()
        {
            var builder = ImmutableDictionary.CreateBuilder<ErrorCode, string>();
            builder.Add(ErrorCode.ReadinessInvalidUrl, "Invalid readiness dependency url");
            builder.Add(ErrorCode.UnhandledException, "Unhandled exception");

            errorMessages = builder.ToImmutable();
        }

        public static string Message(this Error error)
        {
            errorMessages.TryGetValue(error.Code, out var errorMessage);
            var errorMessageBuilder = new StringBuilder(errorMessage);
            if (error.HasAdditionalInfo())
            {
                errorMessageBuilder.Append($".additional info: {error.AdditionalInfo}");
            }

            return errorMessageBuilder.ToString();
        }
    }
}