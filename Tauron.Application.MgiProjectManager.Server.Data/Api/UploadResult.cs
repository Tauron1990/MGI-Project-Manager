using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.Server.Data.Api
{
    public class UploadResult
    {
        public IDictionary<string, string> Errors { get; }

        public string Message { get; }

        public string Operation { get; }

        public UploadResult(IDictionary<string, string> errors, string message, string operation)
        {
            Errors = errors;
            Message = message;
            Operation = operation;
        }
    }
}