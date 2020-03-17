using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using Tauron.Application.Logging;
using Tauron.Application.OptionsStore;
using Tauron.Application.OptionsStore.Store;

namespace Tauron.Application.SimpleAuth.Core
{
    public class PasswordVault : IDisposable
    {
        private const string PasswortName = "Vault-Password";


        private readonly ILogger _logger;
        private readonly object _lock = new object();
        private readonly IDisposable _changeToken;
        private readonly PasswordHasher<string> _hasher;

        private SimplAuthSettings _settings;

        private IAppOptions _appOptions;
        private IOption _password = OptionImpl.EmptyOption;


        // ReSharper disable once SuggestBaseTypeForParameter
        public PasswordVault(IOptionsMonitor<SimplAuthSettings> settings, IOptionsStore optionsStore, ISLogger<PasswordVault> logger)
        {
            _logger = logger;
            _settings = settings.CurrentValue;

            string NameGetter() => $"PasswodVault-{_settings.AppName}";
            _appOptions = optionsStore.GetAppOptions(NameGetter());

            _changeToken = settings.OnChange( parm =>
            {
                _logger.Information("Simple Auth Settings Changed: {Config}", parm.AppName);

                _settings = parm;

                lock (_lock)
                    _appOptions = optionsStore.GetAppOptions(NameGetter());
            });
        }

        public void Dispose() 
            => _changeToken.Dispose();

        public async Task<bool> CheckPassword(string pass)
        {
            using var opt = LogContext.PushProperty("Config", _settings.AppName);

            _logger.Information("Try Check Password");

            try
            {
                var opass = HashPassword(pass);
                var checkPass = _password.Value;
                if (string.IsNullOrWhiteSpace(checkPass))
                    checkPass = HashPassword(_settings.BaseAdminPass);

                return opass == checkPass;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Password Failed to Check");
            }

            return false;
        }

        public Task<bool> SetPassword(string pass)
        {

        }

        private string HashPassword(string pass) 
            => _hasher.HashPassword(PasswortName, pass);
    }
}