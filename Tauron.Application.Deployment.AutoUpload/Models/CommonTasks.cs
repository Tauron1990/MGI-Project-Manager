using System;
using System.Threading.Tasks;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.ToolUI.Login;

namespace Tauron.Application.Deployment.AutoUpload.Models
{
    [ServiceDescriptor]
    public class CommonTasks
    {
        private readonly InputService _inputService;
        private readonly Settings _settings;

        public CommonTasks(InputService inputService, Settings settings)
        {
            _inputService = inputService;
            _settings = settings;
        }

        public async Task<bool> RequestUserInfo()
        {
            var result = await _inputService.Request("User Name", "Bitte User Name eingeben");
            if (string.IsNullOrWhiteSpace(result)) return false;
            _settings.UserName = result;

            result = await _inputService.Request("E-mail", "Bitte E-mail Angeben");
            if (string.IsNullOrWhiteSpace(result)) return false;
            _settings.EMailAdress = result;

            _settings.UserWhen = DateTimeOffset.Now;
            await _settings.Save();

            return true;
        }
    }
}