using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;
using IISServiceManager.Core;
using IISServiceManager.Processing;
using IISServiceManager.Resources;
using Ookii.Dialogs.Wpf;
using TwineSharpEditor.Core.Host;

namespace IISServiceManager
{
    public class MainWindowViewModel : ViewModelBase
    {
        private class DummyWebService : IWebService
        {
            public string Name => "Test Service";
            public string Id => nameof(DummyWebService);
            public string Description => "This is a Test";
            public ServiceType ServiceType => ServiceType.Essential;
        }

        private Dispatcher Dispatcher { get; } = Application.Current.Dispatcher;

        private bool _isBusy;
        private string _progessMessage;
        private ClusterEntry _selectedEntry;
        private bool _showWarning;
        private object _warnignContent;

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public string ProgessMessage
        {
            get => _progessMessage;
            set => Set(ref _progessMessage, value);
        }

        public UIObservableCollection<ClusterEntry> Clusters { get; } = new UIObservableCollection<ClusterEntry>();

        public ClusterEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (Set(ref _selectedEntry, value))
                    LoadServices();
            }
        }

        public InstallEngine InstallEngine { get; }

        public UIObservableCollection<InstallableService> EssentialServices { get; } = new UIObservableCollection<InstallableService>();

        public UIObservableCollection<InstallableService> NormalServices { get; } = new UIObservableCollection<InstallableService>();

        public bool ShowWarning
        {
            get => _showWarning;
            set => Set(ref _showWarning, value);
        }

        public object WarnignContent
        {
            get => _warnignContent;
            set => Set(ref _warnignContent, value);
        }

        public AsyncCommand LoadedCommand { get; }
        
        public AsyncCommand ReCheckPrerequisites { get; }

        public AsyncCommand UpdateAllCommand { get; }

        private AsyncCommand CommonWebServiceClick { get; }

        private AsyncCommand InstallCommand { get; }

        private AsyncCommand UnistallCommand { get; }

        private AsyncCommand UpdateCommand { get; }

        public MainWindowViewModel()
        {
            ProgessMessage = Strings.Progress_Common_Loading;
            InstallEngine = new InstallEngine();

            LoadedCommand = new AsyncCommand(LoadApp);
            ReCheckPrerequisites = new AsyncCommand(CheckPrerequisites, () => SelectedEntry != null);
            CommonWebServiceClick = new AsyncCommand(ExecuteCommonClick);
            InstallCommand = new AsyncCommand(InstallService);
            UnistallCommand = new AsyncCommand(UninstallService);
            UpdateCommand = new AsyncCommand(UpdateService);
            UpdateAllCommand = new AsyncCommand(UpdateAll, () => SelectedEntry != null);
        }

        #region  UI Commands

        private async Task UpdateAll(object arg) 
            => await InstallEngine.UpdateAll();

        private async Task InstallService(object arg)
        {
            if (!(arg is InstallableService service)) return;

            await InstallService(service.WebService);
        }
        private async Task UninstallService(object arg)
        {
            if (!(arg is InstallableService service)) return;

            await UninstallService(service.WebService);
        }
        private async Task UpdateService(object arg)
        {
            if (!(arg is InstallableService service)) return;

            await UpdateService(service.WebService);
        }
        private async Task ExecuteCommonClick(object arg)
        {
            if(!(arg is InstallableService service)) return;

            switch (service.IsInstalled)
            {
                case ServiceStade.Running:
                    await StopService(service.WebService);
                    break;
                case ServiceStade.Stopped:
                    await StartService(service.WebService);
                    break;
                case ServiceStade.NotInstalled:
                    await InstallService(service.WebService);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private async Task LoadApp(object arg)
        {
            IsBusy = true;

            string path = Properties.Settings.Default.WebsitePath;

            while (string.IsNullOrWhiteSpace(path) && !Directory.Exists(path))
            {
                path = await Dispatcher.InvokeAsync(() =>
                                                    {
                                                        var dialog = new VistaFolderBrowserDialog {ShowNewFolderButton = true};

                                                        if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                                                            return dialog.SelectedPath;
                                                        if(MessageBox.Show(Application.Current.MainWindow, Strings.MainWindow_Load_SelectPathError, "Error", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                                            Application.Current.MainWindow.Close();
                                                        return string.Empty;
                                                    });
            }

            var set = Properties.Settings.Default;
            set.WebsitePath = path;
            set.Save();

            foreach (var file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
            {
                try
                {
                    var asm = Assembly.LoadFile(file);

                    foreach (var exportedType in asm.GetExportedTypes())
                    {
                        if (typeof(IWebServiceCluster).IsAssignableFrom(exportedType))
                            Clusters.Add(new ClusterEntry((IWebServiceCluster) Activator.CreateInstance(exportedType)));
                    }
                }
                catch (Exception)
                {
                    // AssemblyLoading not Importend
                }
            }

            IsBusy = false;
        }
        private async Task CheckPrerequisites(object arg)
            => await CheckPrerequisites(_selectedEntry.Cluster);
        #endregion

        private async void LoadServices()
        {
            try
            {
                IsBusy         = true;
                ProgessMessage = Strings.Progress_Common_Loading;

                var root = _selectedEntry.Cluster;

                await CheckPrerequisites(root);

                EssentialServices.Clear();
                NormalServices.Clear();

                await InstallEngine.Initialize(_selectedEntry.Cluster);

                foreach (var service in InstallEngine.Services)
                {
                    service.CommonWebServiceClick = CommonWebServiceClick;
                    service.InstallCommand = InstallCommand;
                    service.UnInstallCommand = UnistallCommand;
                    service.UpdateCommand = UpdateCommand;

                    switch (service.WebService.ServiceType)
                    {
                        case ServiceType.Essential:
                            EssentialServices.Add(service);
                            break;
                        case ServiceType.Normal:
                            NormalServices.Add(service);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                IsBusy = false;
            }
            catch (Exception e)
            {
                ProgessMessage = e.Message;
            }
        }

        private async Task CheckPrerequisites(IWebServiceCluster root)
        {
            var pre = await root.CheckPrerequisites();

            if (pre != null)
            {
                WarnignContent = pre;
                ShowWarning    = true;
            }
        }

        private async Task StopService(IWebService service) 
            => await InstallEngine.StopService(service);

        private async Task StartService(IWebService service) 
            => await InstallEngine.StartService(service);

        private async Task InstallService(IWebService service) 
            => await InstallEngine.InstallService(service);

        private async Task UpdateService(IWebService service)
        {
            await InstallEngine.UpdateRepo();
            await InstallEngine.UpdateSerivce(service);
        }
    }
}