using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.Deployment.Server.CoreApp
{
    public sealed class DatabaseOptions
    {
        private readonly IAppOptions _store;
        private ConcurrentDictionary<string, IOption> _options = new ConcurrentDictionary<string, IOption>();

        public DatabaseOptions(IOptionsStore store) 
            => _store = store.GetAppOptions("DeploymentServer");

        private async Task<string> GetValue(string name)
        {
            if (_options.TryGetValue(name, out var opt))
                return opt.Value;
            
            opt = await _store.GetOption(name);

            _options[name] = opt;
            return opt.Value;
        }

        public async Task SetValue(string name, string value)
        {
            if (_options.TryGetValue(name, out var opt))
            {
                await opt.SetValue(value);
                return;
            }

            opt = await _store.GetOption(name);

            _options[name] = opt;
            await opt.SetValue(value);
        }

        private const string IsSetupFinisht = nameof(IsSetupFinisht);

        public async Task<bool> GetIsSetupFinisht()
        {
            var result = await GetValue(IsSetupFinisht);
            return !string.IsNullOrEmpty(result) && bool.Parse(result);
        }

        public async Task SetIsSetupFinisht(bool value) 
            => await SetValue(IsSetupFinisht, value.ToString());
    }
}