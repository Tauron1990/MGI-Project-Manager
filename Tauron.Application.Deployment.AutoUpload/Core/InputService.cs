    using System.Security;
    using System.Threading.Tasks;
    using Catel.Services;
    using Scrutor;
    using Tauron.Application.Deployment.AutoUpload.Core.UI;

    namespace Tauron.Application.Deployment.AutoUpload.Core
    {
        [ServiceDescriptor]
        public class InputService
        {
            private readonly IDispatcherService _dispatcherService;

            public InputService(IDispatcherService dispatcherService)
                => _dispatcherService = dispatcherService;

            public async Task<string> Request(string caption, string description)
            {
                return await _dispatcherService.InvokeAsync(() =>
                {
                    var diag = new InputDialog {AllowCancel = true, InstructionText = description, MainText = caption};

                    return diag.ShowDialog() == true ? diag.Result : string.Empty;
                });
            }

            public async Task<(string UserName, SecureString Passwort)> Request(string userName)
            {

            }
        }
    }
