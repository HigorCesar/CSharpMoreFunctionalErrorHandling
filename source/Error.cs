using System;
using System.Runtime.CompilerServices;

namespace HandlingOperationResult
{
    public enum ErrorCode
    {
        General,
        ReadinessInvalidUrl,
        UnhandledException,
        Exception,
        InvalidArgument,
        FailedToCommunicateToAws
    }

    public class Error
    {
        public Exception Exception { get; }
        public ErrorCode Code { get; }
        public string Message { get; }
        public string AdditionalInfo { get; }
        public string CallerName { get; }
        public string FilePath { get; }
        public int LineNumber { get; }

        public bool HasAdditionalInfo() => !String.IsNullOrEmpty(AdditionalInfo);
        public bool HasException() => Exception != null;

        public Error(ErrorCode code, string message = "", string additionalInfo = "", [CallerMemberName] string callerName = "",
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0,
            Exception exception = null)
        {
            Code = code;
            Message = message;
            AdditionalInfo = additionalInfo;
            CallerName = callerName;
            FilePath = filePath;
            LineNumber = lineNumber;
            Exception = exception;
        }
    }
}