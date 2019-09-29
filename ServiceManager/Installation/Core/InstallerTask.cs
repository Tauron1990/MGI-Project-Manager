using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ServiceManager.Installation.Core
{
    public abstract class InstallerTask : INotifyPropertyChanged
    {
        private bool _running;

        public abstract object Content { get; }

        public abstract string Title { get; }

        public bool Running
        {
            get => _running;
            set
            {
                if (value == _running) return;
                _running = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task Rollback() => Task.CompletedTask;

        public virtual Task Prepare(InstallerContext context) => Task.CompletedTask;

        public abstract Task<string> RunInstall(InstallerContext context);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}