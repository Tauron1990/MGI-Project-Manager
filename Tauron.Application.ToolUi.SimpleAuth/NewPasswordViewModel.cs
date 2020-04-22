using Catel.MVVM;
using JetBrains.Annotations;
using Scrutor;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    [ServiceDescriptor(typeof(NewPasswordViewModel))]
    [UsedImplicitly]
    public sealed class NewPasswordViewModel : ViewModelBase
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordRepead { get; set; }
    }
}