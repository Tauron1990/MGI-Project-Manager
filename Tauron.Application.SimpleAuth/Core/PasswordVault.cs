using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.SimpleAuth.Core
{
    public class PasswordVault : IDisposable
    {
        private const string PasswortName = "Vault-Password";

        private readonly object _lock = new object();
        private readonly IDisposable _changeToken;
        private readonly PasswordHasher<string> _hasher;

        private SimplAuthSettings _settings;
        private IAppOptions _appOptions;


        public PasswordVault(IOptionsMonitor<SimplAuthSettings> settings, IOptionsStore optionsStore)
        {
            _settings = settings.CurrentValue;

            string NameGetter() => $"PasswodVault-{_settings.AppName}";
            _appOptions = optionsStore.GetAppOptions(NameGetter());

            _changeToken = settings.OnChange( parm =>
            {
                _settings = parm;

                lock (_lock) 
                    _appOptions = optionsStore.GetAppOptions(NameGetter());
            });
        }

        public void Dispose() 
            => _changeToken.Dispose();

        public Task<bool> CheckPassword(string pass)
        {   
            var opass = 
        }

        public Task<bool> SetPassword(string pass)
        {

        }

        private static string HashPassword(string pass)
        {

        }
    }
}