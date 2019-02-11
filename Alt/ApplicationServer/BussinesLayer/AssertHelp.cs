using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [PublicAPI]
    public sealed class AssertHelp
    {
        public AssertHelp(bool ok, string message)
        {
            Ok      = ok;
            Message = message;
        }

        public string Message { get; }

        public bool Ok { get; }
    }
}