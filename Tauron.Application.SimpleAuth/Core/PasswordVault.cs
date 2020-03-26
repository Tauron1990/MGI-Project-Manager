using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using Serilog;
using Serilog.Context;
using Tauron.Application.Logging;
using Tauron.Application.OptionsStore;
using Tauron.Application.OptionsStore.Store;

namespace Tauron.Application.SimpleAuth.Core
{
    public class PasswordVault : IDisposable, IPasswordVault
    {
        public const string PasswortName = "Vault-Password";


        private readonly ILogger _logger;
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        private readonly IDisposable _changeToken;
        private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();

        private SimplAuthSettings _settings;

        private IAppOptions _appOptions;
        private IOption? _password;


        // ReSharper disable once SuggestBaseTypeForParameter
        public PasswordVault(IOptionsMonitor<SimplAuthSettings> settings, IOptionsStore optionsStore, ISLogger<PasswordVault> logger)
        {
            _logger = logger;
            _settings = settings.CurrentValue;

            string NameGetter() => $"PasswodVault-{_settings.AppName}";
            _appOptions = optionsStore.GetAppOptions(NameGetter());

            _changeToken = settings.OnChange(parm =>
            {
                _logger.Information("Simple Auth Settings Changed: {SimpleAuthConfig}", parm.AppName);

                _settings = parm;

                using var l = _lock.WriterLock();

                _appOptions = optionsStore.GetAppOptions(NameGetter());
                _password = null;

            });
        }

        public void Dispose() 
            => _changeToken.Dispose();

        public async Task<bool> CheckPassword(string pass)
        {
            using var opt = LogContext.PushProperty("SimpleAuthConfig", _settings.AppName);

            _logger.Information("Try Check Password");

            try
            {
                var checkPass = await TryGetPassword();
                if (string.IsNullOrWhiteSpace(checkPass))
                    checkPass = HashPassword(_settings.BaseAdminPass);

                var result = _hasher.VerifyHashedPassword(PasswortName, checkPass, pass);
                return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Password Failed to Check");
                throw;
            }
        }

        public async Task<bool> SetPassword(string pass)
        {
            if (string.IsNullOrEmpty(pass))
                return false;

            using var opt = LogContext.PushProperty("SimpleAuthConfig", _settings.AppName);

            var newPass = HashPassword(pass);
            await TryGetPassword();
            if (await CheckPassword(pass))
            {
                _logger.Information("User Try to Set Same Password");

                return false;
            }

            if (_password == null)
            {
                _logger.Warning("Password Option not Found");
                return false;
            }

            await _password.SetValueAsync(newPass);
            _logger.Information("Password in Simple Auth Changed");

            return true;
        }

        private async Task<string> TryGetPassword()
        {
            using (await _lock.ReaderLockAsync())
            {
                if (_password != null)
                    return _password.Value;

                using var opt = LogContext.PushProperty("SimpleAuthConfig", _settings.AppName);

                _logger.Information("Try acquire password option");

                _password = await _appOptions.GetOptionAsync(PasswortName);

                return _password.Value;
            }
        }

        private string HashPassword(string pass) 
            => _hasher.HashPassword(PasswortName, pass);
    }
}