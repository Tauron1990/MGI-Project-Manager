using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ServiceManager.Annotations;
using ServiceManager.Core;

namespace ServiceManager
{
    public class MainWindowsModel : INotifyPropertyChanged
    {
        private static readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron\\ServiceManager\\Settings.json");
        private RunningServices _runningServices;
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


        public async Task BeginLoad()
        {
            var dic = Path.GetDirectoryName(_settingsPath);

            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);

            _runningServices = await RunningServices.Read(_settingsPath);
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}