using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ServiceManager.Services
{
    public sealed class RunningService : INotifyPropertyChanged
    {
        private string _installationPath;
        private ServiceStade _serviceStade;
        private string _name;
        private string _exe;

        public string InstallationPath
        {
            get => _installationPath;
            set
            {
                if (value == _installationPath) return;
                _installationPath = value;
                OnPropertyChanged();
            }
        }

        public ServiceStade ServiceStade
        {
            get => _serviceStade;
            set
            {
                if (value == _serviceStade) return;
                _serviceStade = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Exe
        {
            get => _exe;
            set => _exe = value;
        }

        public RunningService(string installationPath, ServiceStade serviceStade, string name, string exe)
        {
            InstallationPath = installationPath;
            ServiceStade = serviceStade;
            Name = name;
            Exe = exe;
        }

        public RunningService()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}