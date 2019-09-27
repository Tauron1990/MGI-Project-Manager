using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nito.AsyncEx;
using ServiceManager.Core;

namespace ServiceManager.Installation.Tasks.Ui
{
    public sealed class NameSelectionModel : INotifyPropertyChanged
    {
        private readonly AsyncManualResetEvent _manualResetEvent = new AsyncManualResetEvent(false);
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

        public NameSelectionModel(ServiceSettings serviceSettings) 
            => NameList = new ObservableCollection<string>(serviceSettings.RunningServices.Select(r => r.Name));

        public Task Wait() => _manualResetEvent.WaitAsync();

        public void Click() => _manualResetEvent.Set();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}