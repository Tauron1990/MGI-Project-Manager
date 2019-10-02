using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using ServiceManager.ProcessManager;
using ServiceManager.Services;

namespace ServiceManager
{
    public class UiService : INotifyPropertyChanged
    {
        private readonly IProcessManager _processManager;
        private bool _canClick;
        private bool _progress;

        public RunningService Service { get; }

        public ICommand Click { get; }

        public bool CanClick
        {
            get => _canClick;
            set
            {
                if (value == _canClick) return;
                _canClick = value;
                OnPropertyChanged();
            }
        }

        public bool Progress
        {
            get => _progress;
            set => _progress = value;
        }

        public UiService(RunningService service, IProcessManager processManager)
        {
            _processManager = processManager;
            Progress = false;
            CanClick = true;

            Click = new ActionCommand(ClickMethod);

            Service = service;
        }

        private async Task ClickMethod()
        {
            try
            {
                switch (Service.ServiceStade)
                {
                    case ServiceStade.Error:
                        break;
                    case ServiceStade.Running:
                        break;
                    case ServiceStade.Ready:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                CanClick = false;
                Progress = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class ActionCommand : ICommand
        {
            private readonly Func<Task> _runner;

            public ActionCommand(Func<Task> runner)
            {
                _runner = runner;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
                => await _runner();

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }
        }

    }
}