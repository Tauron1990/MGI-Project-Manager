using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.Deployment.AutoUpload
{
    public sealed class UploadLogContext : LogContextBase
    {
        protected override object ContextData() => "Upload-Tool";
    }
}