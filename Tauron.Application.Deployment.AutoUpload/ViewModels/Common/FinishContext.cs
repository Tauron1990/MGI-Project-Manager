using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public sealed class FinishContext : OperationContextBase
    {
        public string Message { get; }

        public FinishContext(string? message) 
            => Message = message ?? "Die Operation wurde Erfolgreich Beendet";
    }
}