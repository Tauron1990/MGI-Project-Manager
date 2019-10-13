using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ServiceManager.Installation.Core
{
    public abstract class InstallerTask : INotifyPropertyChanged, IDisposable
    {
        public static class MetaKeys
        {
            public const string UpdateFile = nameof(UpdateFile);

            public const string ArchiveFile = nameof(ArchiveFile);

            public const string TempLocation = nameof(TempLocation);
        }

        private bool _running;
        private object _content;

        public object Content
        {
            get => _content;
            protected set
            {
                if (Equals(value, _content)) return;
                _content = value;
                OnPropertyChanged();
            }
        }

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

        public virtual Task Rollback(InstallerContext context) => Task.CompletedTask;

        public virtual Task Prepare(InstallerContext context) => Task.CompletedTask;

        public abstract Task<string> RunInstall(InstallerContext context);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public virtual void Dispose()
        {
        }
    }
}