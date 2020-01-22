using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.Deployment.Server.CoreApp.Server
{
    public sealed class DatabaseOptions : INotifyPropertyChanged
    {
        private readonly IAppOptions _store;
        private readonly ConcurrentDictionary<string, IOption> _options = new ConcurrentDictionary<string, IOption>();

        public DatabaseOptions(IOptionsStore store) 
            => _store = store.GetAppOptions("DeploymentServer");

        private async Task<string> GetValueAsync(string name)
        {
            if (_options.TryGetValue(name, out var opt))
                return opt.Value;
            
            opt = await _store.GetOptionAsync(name);

            _options[name] = opt;
            return opt.Value;
        }

        private async Task SetValueAsync(string name, string value)
        {
            if (_options.TryGetValue(name, out var opt))
            {
                await opt.SetValueAsync(value);
                return;
            }

            opt = await _store.GetOptionAsync(name);

            _options[name] = opt;
            await opt.SetValueAsync(value);
        }


        private string GetValue(string name)
        {
            if (_options.TryGetValue(name, out var opt))
                return opt.Value;

            opt = _store.GetOption(name);

            _options[name] = opt;
            return opt.Value;
        }

        private void SetValue(string name, string value)
        {
            if (_options.TryGetValue(name, out var opt))
            {
                opt.SetValue(value);
                return;
            }

            opt = _store.GetOption(name);

            _options[name] = opt;
            opt.SetValue(value);
        }


        private const string IsSetupFinisht = nameof(IsSetupFinisht);

        public async Task<bool> GetIsSetupFinisht()
        {
            var result = await GetValueAsync(IsSetupFinisht);
            return !string.IsNullOrEmpty(result) && bool.Parse(result);
        }

        public async Task SetIsSetupFinisht(bool value) 
            => await SetValueAsync(IsSetupFinisht, value.ToString());

        public ServerFileMode ServerFileMode
        {
            get { return Enum.TryParse<ServerFileMode>(GetValue(nameof(ServerFileMode)), out var serverFileMode) ? serverFileMode : ServerFileMode.Unkowen; }
            set
            {
                SetValue(nameof(ServerFileMode), value.ToString());
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}