using System;
using System.Threading.Tasks;
using Anotar.Serilog;
using Catel.MVVM;
using JetBrains.Annotations;
using Scrutor;
using Serilog.Context;
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
            using (LogContext.PushProperty("Host", TargetHost))
            {
                IsRunning = true;
                try
                {
                    LogTo.Information("Set new Passwort");

                    if (!Uri.TryCreate(TargetHost, UriKind.Absolute, out _))
                    {
                        LogTo.Warning("Wrong Host Uri Format");
                        Error = "Falsches Uri Format";
                        return;
                    }

                    LogTo.Information("Sending SetPassword Request");

                    var client = _loginService.Create(TargetHost);
                    var result = await client.SetpasswordAsync(new NewPasswordData
                    {
                        NewPassword = NewPassword,
                        OldPassword = OldPassword
                    });
                    
                    switch (result.ResultCase)
                    {
                        case SetPasswordResult.ResultOneofCase.None:
                            Error = "Unbekannter Fehler";
                            LogTo.Warning("Unkown Error on Set Password");
                            break;
                        case SetPasswordResult.ResultOneofCase.Token:
                            _manager.SetToken(TargetHost, result.Token);
                            Error = "Erfolgreich";
                            LogTo.Information("Set Password Compled");
                            break;
                        case SetPasswordResult.ResultOneofCase.Status:
                            Error = result.Status.ToString();
                            LogTo.Warning("{Error} on set Password", result.Status.ToString());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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