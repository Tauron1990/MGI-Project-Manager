using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ServiceManager.Core
{
    public sealed class ServiceNameEnteringWindowModel : INotifyPropertyChanged
    {
        private string _nameText;

        public string NameText
        {
            get => _nameText;
            set
            {
                if (value == _nameText) return;
                _nameText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> NameList { get; }

        public ServiceNameEnteringWindowModel(ServiceSettings serviceSettings) 
            => NameList = new ObservableCollection<string>(serviceSettings.RunningServices.Select(r => r.Name));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}