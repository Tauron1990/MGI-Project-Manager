using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using Akka.Actor;
using Akka.Copy.Actors;
using Akka.Copy.App.Core;
using Akka.Copy.Messages;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;

namespace Akka.Copy.App
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _currentWorkingDictionary = string.Empty;
        private int _tabIndex;
        private CancellationTokenSource _cancellationToken;

        private readonly IEventActor _copySystemInBox;
        private readonly IActorRef _copySystem;

        public event PropertyChangedEventHandler PropertyChanged;

        public CopySettings Settings { get; } = new CopySettings();

        public ProgressTracker Tracker { get; } = new ProgressTracker();
        
        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                if (value == _tabIndex) return;
                _tabIndex = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            var system = CoreSystem.System;

            _copySystem = system.ActorOf(StartCopyActor.CreateProps(system), "Start-Copy");
            _copySystemInBox = EventActor.Create(system);
            _copySystemInBox.Watch(_copySystem);

            _copySystemInBox.Register(HookEvent.Create<UpdateMessage>(UpdateMessage));
            _copySystemInBox.Register(HookEvent.Create<NewFileCopyStartMessage>(NewFileCopyStartMessage));
            _copySystemInBox.Register(HookEvent.Create<RecuverErrorMessage>(RecuverErrorMessage));
            _copySystemInBox.Register(HookEvent.Create<CopyCompledMessage>(CopyCompledMessage));
        }

        private void CopyCompledMessage(CopyCompledMessage obj)
        {
            Tracker.Reset();
            Tracker.ProgressStade = "Fertig";
        }

        private void RecuverErrorMessage(RecuverErrorMessage obj) 
            => Tracker.Log.Add(obj.Error.Message);

        private string _file1;
        private string _file2;
        private string _file3;
        private string _file4;

        private void NewFileCopyStartMessage(NewFileCopyStartMessage obj)
        {
            if (_file1 == obj.Id)
                Tracker.File1 = obj.Name;
            else if (_file2 == obj.Id)
                Tracker.File2 = obj.Name;
            else if (_file3 == obj.Id)
                Tracker.File3 = obj.Name;
            else if (_file4 == obj.Id)
                Tracker.File4 = obj.Name;
            else
            {
                if (_file1 == null)
                {
                    _file1 = obj.Id;
                    Tracker.File1 = obj.Name;
                }
                else if (_file2 == null)
                {
                    _file2 = obj.Id;
                    Tracker.File2 = obj.Name;
                }
                else if (_file3 == null)
                {
                    _file3 = obj.Id;
                    Tracker.File3 = obj.Name;
                }
                else if (_file4 == null)
                {
                    _file4 = obj.Id;
                    Tracker.File4 = obj.Name;
                }
            }
        }

        private int _update;

        private void UpdateMessage(UpdateMessage obj)
        {
            if(Interlocked.Increment(ref _update) != 10)
                return;

            Interlocked.Exchange(ref _update, 0);

            Tracker.ProgressStade = obj.Message;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (obj.Percent == -1)
                Tracker.Percent = 0;
            else
                Tracker.Percent = obj.Percent;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void SearchBase()
        {
            var path = SearchDic();
            if(path == null) return;

            Settings.Base = path;
        }

        private string SearchDic()
        {
            var diag = new VistaFolderBrowserDialog
            {
                SelectedPath = _currentWorkingDictionary,
                ShowNewFolderButton = true,
                Description = @"Bitte Pfad Wählen"
            };

            if (diag.ShowDialog(Application.Current.MainWindow) != true) return null;

            var temp = diag.SelectedPath;
            _currentWorkingDictionary = temp;
            Environment.CurrentDirectory = temp;

            return temp;
        }

        public void AddDics(IEnumerable<string> dics)
        {
            foreach (var dic in dics.OrderBy(s => s).Select(d =>
                                            {
                                                if(Directory.Exists(d))
                                                    return new DirectoryInfo(d).Name;
                                                return File.Exists(d) ? new FileInfo(d).Name : string.Empty;
                                            }))
            {
                if (Settings.Entrys.Contains(dic) || string.IsNullOrEmpty(dic)) continue;

                Settings.Entrys.Add(dic);
            }
        }

        public void SearchTarget()
        {
            var path = SearchDic();
            if (path == null) return;

            Settings.Target = path;
        }

        public void StartProgress()
        {
            Tracker.IsLocked = true;
            Tracker.Log.Clear();

            _file1 = null;
            _file2 = null;
            _file3 = null;
            _file4 = null;

            _update = 0;

            _cancellationToken?.Dispose();
            _cancellationToken = new CancellationTokenSource();

            Settings.Save();
            TabIndex = 1;
            
            _copySystemInBox.Send(_copySystem, new StartCopyMessage(Settings.Base, Settings.Target, Settings.Entrys.ToArray(), _cancellationToken.Token));
        }

        public void LoadLast() 
            => Settings.Load();

        public void Stop() => _cancellationToken.Cancel();
    }
}