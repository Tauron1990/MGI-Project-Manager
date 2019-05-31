using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;
using IISServiceManager.Core;

namespace IISServiceManager
{
    /// <summary>
    /// Interaktionslogik für LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private class LogModel : ViewModelBase, ILog
        {
            private readonly LogWindow _logWindow;
            private readonly ManualResetEvent _manualReset = new ManualResetEvent(true);
            private bool _autoClose = true;

            public LogModel(LogWindow logWindow)
            {
                _logWindow = logWindow;
                _logWindow.Closed += (sender, args) => _manualReset.Set();
            }

            public UIObservableCollection<string> Lines { get; } = new UIObservableCollection<string>();

            public bool AutoClose
            {
                get => _autoClose;
                set
                {
                    if (!Set(ref _autoClose, value)) return;

                    if (value) _manualReset.Set();
                    else _manualReset.Reset();
                }
            }

            public void WriteLine(string content) => Lines.Add(content);
            
            public async Task EnterOperation() 
                => await _logWindow.Dispatcher.InvokeAsync(() => _logWindow.Show());

            public Task ExitOperation()
            {
                if (AutoClose)
                {
                    _logWindow.Dispatcher.InvokeAsync(() => _logWindow.Close());
                    return Task.CompletedTask;
                }

                _manualReset.WaitOne();
                return Task.CompletedTask;
            }

            public void Dispose() 
                => _manualReset.Dispose();
        }

        public LogWindow(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            Log = new LogModel(this);
            DataContext = Log;
        }

        public ILog Log { get; }
    }
}
