using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Akka.Copy.App.Helper;
using JetBrains.Annotations;

namespace Akka.Copy.App.Core
{
    public sealed class ProgressTracker : INotifyPropertyChanged
    {
        private bool _isNotLocked;
        private double _percent;
        private string _file1;
        private string _file2;
        private string _file3;
        private string _file4;
        private string _progressStade;
        private bool _isLocked;

        public bool IsNotLocked
        {
            get => _isNotLocked;
            set
            {
                if (value == _isNotLocked) return;
                _isNotLocked = value;
                OnPropertyChanged();
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (value == _isLocked) return;
                _isLocked = value;
                IsNotLocked = !value;
                OnPropertyChanged();
            }
        }

        public double Percent
        {
            get => _percent;
            set
            {
                if (value.Equals(_percent)) return;
                Interlocked.Exchange(ref _percent, value);
                OnPropertyChanged();
            }
        }

        public string File1
        {
            get => _file1;
            set
            {
                if (value == _file1) return;
                _file1 = value;
                OnPropertyChanged();
            }
        }

        public string File2
        {
            get => _file2;
            set
            {
                if (value == _file2) return;
                _file2 = value;
                OnPropertyChanged();
            }
        }

        public string File3
        {
            get => _file3;
            set
            {
                if (value == _file3) return;
                _file3 = value;
                OnPropertyChanged();
            }
        }

        public string File4
        {
            get => _file4;
            set
            {
                if (value == _file4) return;
                _file4 = value;
                OnPropertyChanged();
            }
        }

        public string ProgressStade
        {
            get => _progressStade;
            set
            {
                if (value == _progressStade) return;
                _progressStade = value;
                OnPropertyChanged();
            }
        }

        public UiObservableCollection<string> Log { get; } = new UiObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ProgressTracker()
        {
            ProgressStade = "Warte auf Start";
            Reset();
        }

        public void Reset()
        {
            IsLocked = false;
            IsNotLocked = true;
            const string noFile = "Keine Datei";
            File1 = noFile;
            File2 = noFile;
            File3 = noFile;
            File4 = noFile;
            Percent = 0;
        }
    }
}