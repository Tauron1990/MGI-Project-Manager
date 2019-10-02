using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.ApiRequester;
using ServiceManager.Core;
using ServiceManager.Installation;
using ServiceManager.ProcessManager;
using Application = System.Windows.Application;
using IWin32Window = System.Windows.Forms.IWin32Window;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ServiceManager
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class MainWindowsModel : INotifyPropertyChanged
    {
        private const string ServiceName = "Service_Manager";
        public static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron\\ServiceManager\\Settings.json");
        private readonly IApiRequester _apiRequester;
        private readonly Dispatcher _dispatcher;
        private readonly IInstallerSystem _installerSystem;
        private readonly IProcessManager _processManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ServiceSettings _serviceSettings;
        private bool _isReady;
        private UiService _selectedService;

        public MainWindowsModel(LogEntries logEntries, ServiceSettings serviceSettings, Dispatcher dispatcher, IServiceScopeFactory serviceScopeFactory, IApiRequester apiRequester,
                                IInstallerSystem installerSystem, IProcessManager processManager)
        {
            _serviceSettings = serviceSettings;
            _dispatcher = dispatcher;
            _serviceScopeFactory = serviceScopeFactory;
            _apiRequester = apiRequester;
            _installerSystem = installerSystem;
            _processManager = processManager;
            LogEntries = logEntries;
        }

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

        public ObservableCollection<UiService> RunningServices { get; } = new ObservableCollection<UiService>();

        public UiService SelectedService
        {
            get => _selectedService;
            set
            {
                if (Equals(value, _selectedService)) return;
                _selectedService = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task BeginLoad()
        {
            var dic = Path.GetDirectoryName(SettingsPath);

            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);

            Uri targetUri = null;


            while (string.IsNullOrWhiteSpace(_serviceSettings.Url) || !Uri.TryCreate(_serviceSettings.Url, UriKind.RelativeOrAbsolute, out targetUri))
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var window = scope.ServiceProvider.GetRequiredService<ValueRequesterWindow>();
                window.MessageText = "Bitte Adresse des Dispatchers Eingeben.";

                if (await _dispatcher.InvokeAsync(window.ShowDialog) == true)
                {
                    _serviceSettings.Url = window.Result;
                    await ServiceSettings.Write(_serviceSettings, SettingsPath);
                    break;
                }

                if (!window.Shutdown) continue;

                if (Application.Current.Dispatcher != null)
                    await Application.Current.Dispatcher.InvokeAsync(Application.Current.Shutdown);
            }

            if (string.IsNullOrWhiteSpace(_serviceSettings.ApiKey))
            {
                try
                {
                    var key = await _apiRequester.RegisterApiKey(ServiceName);
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        MessageBox.Show("Fehler beim Anfordern eines Api Keys.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (Application.Current.Dispatcher != null)
                            await Application.Current.Dispatcher.InvokeAsync(Application.Current.Shutdown);
                    }

                    _serviceSettings.ApiKey = key;
                    await ServiceSettings.Write(_serviceSettings, SettingsPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Fehler: {e}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }

            App.ClientCofiguration.SetUrls(targetUri, ServiceName, _serviceSettings.ApiKey);

            foreach (var runningService in _serviceSettings.RunningServices)
                RunningServices.Add(new UiService(runningService, _processManager));

            IsReady = true;
        }

        public async Task Install()
        {
            var folderBrowser = new OpenFileDialog {AutoUpgradeEnabled = true};

            folderBrowser.ShowDialog(new Wind32Proxy(Application.Current.MainWindow));

            var result = await _installerSystem.Install(folderBrowser.FileName);

            if (result == null) return;

            RunningServices.Add(new UiService(result, _processManager));
            _serviceSettings.RunningServices.Add(result);

            await ServiceSettings.Write(_serviceSettings, SettingsPath);
        }

        public async Task UnInstall()
        {
            if (SelectedService == null) return;

            await _installerSystem.Unistall(SelectedService.Service);
            _serviceSettings.RunningServices.Remove(SelectedService.Service);

            RunningServices.Remove(SelectedService);
            SelectedService = null;

            await ServiceSettings.Write(_serviceSettings, SettingsPath);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class Wind32Proxy : IWin32Window
        {
            public Wind32Proxy(Window window)
            {
                var handle = new WindowInteropHelper(window);

                Handle = handle.EnsureHandle();
            }

            public IntPtr Handle { get; }
        }
    }
}