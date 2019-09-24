using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using ServiceManager.Core;

namespace ServiceManager
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class MainWindowsModel : INotifyPropertyChanged
    {
        public static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron\\ServiceManager\\Settings.json");
        private ServiceSettings _serviceSettings;
        private bool _isReady;

        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (value == _isReady) return;
                _isReady = value;
                OnPropertyChanged();
            }
        }

        public LogEntries LogEntries { get; }

        public MainWindowsModel(LogEntries logEntries) 
            => LogEntries = logEntries;

        public async Task BeginLoad()
        {
            var dic = Path.GetDirectoryName(SettingsPath);

            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);

            _serviceSettings = await ServiceSettings.Read(SettingsPath);
            Uri targetUri = null;


            while (string.IsNullOrWhiteSpace(_serviceSettings.Url) || !Uri.TryCreate(_serviceSettings.Url, UriKind.RelativeOrAbsolute, out targetUri))
            {
                var window = new ValueRequesterWindow("Bitte Adresse des Dispatchers Eingeben.");

                if (window.ShowDialog() == true)
                {
                    _serviceSettings.Url = window.Result;
                    await ServiceSettings.Write(_serviceSettings, SettingsPath);
                    break;
                }

                if (!window.Shutdown) continue;

                if (Application.Current.Dispatcher != null) 
                    await Application.Current.Dispatcher.InvokeAsync(Application.Current.Shutdown);
                return;
            }


            
            App.ClientCofiguration.SetUrls(targetUri, "ServiceManager", _serviceSettings.ApiKey);

            IsReady = true;
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}