using System.Collections.Concurrent;
using System.Security;
using System.Threading.Tasks;
using Catel.Services;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core.UI;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    [ServiceDescriptor]
    [UsedImplicitly]
    public class InputService
    {
        private readonly CredinalDataStore _dataStore;

        private readonly IDispatcherService _dispatcherService;
        private readonly ConcurrentDictionary<string, UserCredinals> _userCredinals = new ConcurrentDictionary<string, UserCredinals>();

        public InputService(IDispatcherService dispatcherService, CredinalDataStore dataStore)
        {
            _dispatcherService = dispatcherService;
            _dataStore = dataStore;
        }

        public async Task<string> Request(string caption, string description)
        {
            return await _dispatcherService.InvokeAsync(() =>
            {
                var diag = new InputDialog
                {
                    AllowCancel = true,
                    InstructionText = description,
                    MainText = caption
                };

                return diag.ShowDialog() == true ? diag.Result : string.Empty;
            });
        }

        //public string GetToken(string userName)
        //{

        //}

        public (string? UserName, SecureString? Passwort) Request(string userName)
        {
            if (userName == null)
                userName = string.Empty;
            if (_userCredinals.TryGetValue(userName, out var credinals))
                return (credinals.UserName, credinals.Password);

            (string? UserName, SecureString? Passwort) result = default;

            var pass = _dataStore.Get(userName);
            if (string.IsNullOrWhiteSpace(pass))
            {
                _dispatcherService.Invoke(() =>
                {
                    var window = new UserNamePasswordRequesterWindow {UserName = userName};
                    if (window.ShowDialog() == true)
                        result = (window.UserName, window.Password);
                });
            }

            if (result.Passwort != null && result.UserName != null)
                _userCredinals[userName] = new UserCredinals(result.Passwort, result.UserName);
            return result;
        }

        public void DeleteCredinals(string name)
        {
            _userCredinals.TryRemove(name, out _);
        }

        private class UserCredinals
        {
            public UserCredinals(SecureString password, string userName)
            {
                Password = password;
                UserName = userName;
            }

            public SecureString Password { get; }

            public string UserName { get; }
        }
    }
}