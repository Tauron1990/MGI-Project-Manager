#region

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The wpf window.</summary>
    public sealed class WpfWindow : IWindow, IDisposable
    {
        #region Public Events

        /// <summary>The closed.</summary>
        public event EventHandler Closed
        {
            add => _window.Closed += value;

            remove => _window.Closed -= value;
        }

        #endregion

        #region Fields

        private readonly Window _window;

        private bool _disposed;

        private HwndSource _source;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfWindow" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WpfWindow" /> Klasse.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public WpfWindow([NotNull] Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));
            _window = window;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="WpfWindow" /> class.
        ///     Finalisiert eine Instanz der <see cref="WpfWindow" /> Klasse.
        /// </summary>
        ~WpfWindow()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the handle.</summary>
        public IntPtr Handle
        {
            get
            {
                EnsureSource();
                return _source.Handle;
            }
        }

        public bool? DialogResult
        {
            get { return UiSynchronize.Synchronize.Invoke(() => _window.DialogResult); }
            set { UiSynchronize.Synchronize.Invoke(() => _window.DialogResult = value); }
        }

        /// <summary>Gets or sets the title.</summary>
        public string Title
        {
            get { return UiSynchronize.Synchronize.Invoke(() => _window.Title); }

            set { UiSynchronize.Synchronize.Invoke(() => _window.Title = value); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public bool? IsVisible
        {
            get
            {
                if (_disposed) return null;
                return _window.Dispatcher.Invoke(() => _window.IsVisible);
            }
        }

        /// <summary>
        ///     The add hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        public void AddHook(WindowHook winProc)
        {
            if (winProc == null) throw new ArgumentNullException(nameof(winProc));
            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.AddHook(Create(winProc)));
        }

        /// <summary>The close.</summary>
        public void Close()
        {
            UiSynchronize.Synchronize.Invoke(() =>
            {
                Dispose();
                _window.Close();
            });
        }

        /// <summary>
        ///     The remove hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        public void RemoveHook(WindowHook winProc)
        {
            if (winProc == null) throw new ArgumentNullException(nameof(winProc));
            EnsureSource();
            UiSynchronize.Synchronize.Invoke(() => _source.RemoveHook(Create(winProc)));
        }

        /// <summary>The show.</summary>
        public void Show()
        {
            UiSynchronize.Synchronize.Invoke(() =>
            {
                var info = _window.DataContext as IShowInformation;
                info?.OnShow(this);

                _window.Show();
            });
        }

        public Task ShowDialogAsync(IWindow window)
        {
            return UiSynchronize.Synchronize.BeginInvoke(() =>
            {
                _window.Owner = window?.TranslateForTechnology() as Window;

                var info = _window.DataContext as IShowInformation;
                info?.OnShow(this);

                _window.ShowDialog();
            });
        }

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object TranslateForTechnology()
        {
            return _window;
        }

        public void Focus()
        {
            UiSynchronize.Synchronize.BeginInvoke(() => _window.Focus());
        }

        public void Hide()
        {
            UiSynchronize.Synchronize.Invoke(() => _window.Hide());
        }

        public object Result
        {
            get
            {
                return UiSynchronize.Synchronize.Invoke(() =>
                {
                    var temp = _window.DataContext as IResultProvider;
                    return temp?.Result;
                });
            }
        }

        #endregion

        #region Methods

        private void CheckDiposed()
        {
            if (_disposed) throw new ObjectDisposedException(ToString());
        }

        [NotNull]
        private HwndSourceHook Create([NotNull] WindowHook hook)
        {
            return (HwndSourceHook) Delegate.CreateDelegate(typeof(HwndSourceHook), hook.Target, hook.Method);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);

            _source?.Dispose();

            _disposed = true;
        }

        private void EnsureSource()
        {
            CheckDiposed();
            if (_source == null) _source = HwndSource.FromHwnd(new WindowInteropHelper(_window).Handle);
        }

        #endregion
    }
}