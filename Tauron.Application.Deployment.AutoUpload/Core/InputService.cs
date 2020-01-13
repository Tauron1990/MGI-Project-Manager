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
            private class UserCredinals
            {
                public SecureString Password { get; }

                public string UserName { get; }

                public UserCredinals(SecureString password, string userName)
                {
                    Password = password;
                    UserName = userName;
                }
            }

            private readonly IDispatcherService _dispatcherService;
            private ConcurrentDictionary<string, UserCredinals> _userCredinals = new ConcurrentDictionary<string, UserCredinals>();

            public InputService(IDispatcherService dispatcherService)
                => _dispatcherService = dispatcherService;

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

            public (string? UserName, SecureString? Passwort) Request(string userName)
            {
                if (userName == null)
                    userName = string.Empty;
                if (_userCredinals.TryGetValue(userName, out var credinals))
                    return (credinals.UserName, credinals.Password);

                (string? UserName, SecureString? Passwort) result = default;

                _dispatcherService.Invoke(() =>
                                          {
                                              var window = new UserNamePasswordRequesterWindow {UserName = userName};
                                              if (window.ShowDialog() == true)
                                                  result = (window.UserName, window.Password);
                                          });

                if (result.Passwort != null && result.UserName != null)
                    _userCredinals[userName] = new UserCredinals(result.Passwort, result.UserName);
                return result;
            }

            public void DeleteCredinals(string name) 
                => _userCredinals.TryRemove(name, out _);
        }
    }
