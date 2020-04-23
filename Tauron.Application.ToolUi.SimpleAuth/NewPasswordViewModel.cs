using System;
using System.Threading.Tasks;
using Catel.MVVM;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.SimpleAuth.Api.Proto;
using Tauron.Application.ToolUi.SimpleAuth.Client;
using Tauron.Application.Wpf;

using static Tauron.Application.SimpleAuth.Api.Proto.LoginService;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    [ServiceDescriptor(typeof(NewPasswordViewModel))]
    [UsedImplicitly, PublicAPI]
    public sealed class NewPasswordViewModel : ViewModelBase
    {
        private readonly IClientFactory<LoginServiceClient> _loginService;
        private readonly ChannelManager _manager;

        public NewPasswordViewModel(IClientFactory<LoginServiceClient> loginService, ChannelManager manager)
        {
            _loginService = loginService;
            _manager = manager;
            ValidateModelsOnInitialization = true;
        }

        public string TargetHost { get; set; }

        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string NewPasswordRepead { get; set; } = string.Empty;

        public string CurrentState { get; set; } = "Senden";

        public string Error { get; set; } = string.Empty;

        public bool IsRunning { get; set; }

        [CommandTarget]
        public bool CanSetPassword() => !HasErrors;

        [CommandTarget]
        public async Task SetPassword()
        {
            IsRunning = true;
            try
            {
                if (!Uri.TryCreate(TargetHost, UriKind.Absolute, out _))
                {
                    Error = "Falsches Uri Format";
                    return;
                }

                var client = _loginService.Create(TargetHost);

                var result = await client.SetpasswordAsync(new NewPasswordData
                                                           {
                                                               NewPassword = NewPassword,
                                                               OldPassword = OldPassword
                                                           });
                

            }
            catch (Exception e)
            {
                IsRunning = false;
                Error = e.Message;
            }
            finally
            {
                Clear();
            }
        }

        private void Clear()
        {
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            NewPasswordRepead = string.Empty;

            IsRunning = false;
        }
    }
}