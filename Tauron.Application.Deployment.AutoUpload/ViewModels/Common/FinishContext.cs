using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public sealed class FinishContext : OperationContextBase
    {
        public static readonly FinishContext Default = new FinishContext();

        public FinishContext(string? message = null)
        {
            Message = message ?? "Die Operation wurde Erfolgreich Beendet";
        }

        public string Message { get; }
    }
}