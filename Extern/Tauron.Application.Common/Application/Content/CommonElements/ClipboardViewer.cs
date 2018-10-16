#region

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using Tauron.Interop;

#endregion

namespace Tauron.Application
{
    /// <summary>The clipboard viewer.</summary>
    [PublicAPI]
    public sealed class ClipboardViewer : IDisposable
    {
        #region Public Events

        /// <summary>The clipboard changed.</summary>
        public event EventHandler ClipboardChanged;

        #endregion

        private class ViewerSafeHandle : SafeHandleMinusOneIsInvalid
        {
            #region Fields

            private readonly IWindow _current;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ViewerSafeHandle" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ViewerSafeHandle" /> Klasse.
            /// </summary>
            /// <param name="nextViewer">
            ///     The next viewer.
            /// </param>
            /// <param name="current">
            ///     The current.
            /// </param>
            public ViewerSafeHandle(IntPtr nextViewer, [NotNull] IWindow current)
                : base(true)
            {
                _current = current;
                SetHandle(nextViewer);
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The set newhandler.
            /// </summary>
            /// <param name="intPtr">
            ///     The int ptr.
            /// </param>
            public void SetNewhandler(IntPtr intPtr)
            {
                SetHandle(intPtr);
            }

            #endregion

            #region Methods

            /// <summary>The release handle.</summary>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            protected override bool ReleaseHandle()
            {
                return NativeMethods.ChangeClipboardChain(_current.Handle, DangerousGetHandle());
            }

            #endregion
        }

        #region Fields

        /// <summary>The _disposed.</summary>
        private bool _disposed;

        /// <summary>The _h wnd next viewer.</summary>
        private ViewerSafeHandle _hWndNextViewer;

        /// <summary>The _is viewing.</summary>
        private bool _isViewing;

        /// <summary>The _target.</summary>
        [CanBeNull] private IWindow _target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClipboardViewer" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ClipboardViewer" /> Klasse.
        ///     Initializes a new instance of the <see cref="ClipboardViewer" /> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        public ClipboardViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            _target = target;
            if (registerForClose) _target.Closed += TargetClosed;

            if (performInitialization) Initialize();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="ClipboardViewer" /> class.
        ///     Finalisiert eine Instanz der <see cref="ClipboardViewer" /> Klasse.
        ///     Finalizes an instance of the <see cref="ClipboardViewer" /> class.
        /// </summary>
        ~ClipboardViewer()
        {
            Dispose();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summa
        public void Dispose()
        {
            if (_disposed) return;

            CloseCbViewer();
            if (_target != null) _target.Closed -= TargetClosed;
            _target = null;

            ClipboardChanged = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>The initialize.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (_isViewing) return;

            if (_target != null)
            {
                _target.AddHook(WinProc); // start processing window messages
                _hWndNextViewer = new ViewerSafeHandle(
                    NativeMethods.SetClipboardViewer(_target.Handle),
                    _target); // set this window as a viewer
            }

            _isViewing = true;
        }

        #endregion

        #region Methods

        /// <summary>The close cb viewer.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CloseCbViewer()
        {
            if (!_isViewing) return;

            // remove this window from the clipboard viewer chain
            _hWndNextViewer.Dispose();

            _hWndNextViewer = null;
            if (_target != null) _target.RemoveHook(WinProc);
            _isViewing = false;
        }

        /// <summary>The on clipboard changed.</summary>
        private void OnClipboardChanged()
        {
            var handler = ClipboardChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The target closed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void TargetClosed([NotNull] object sender, [NotNull] EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        ///     The win proc.
        /// </summary>
        /// <param name="hwnd">
        ///     The hwnd.
        /// </param>
        /// <param name="msg">
        ///     The msg.
        /// </param>
        /// <param name="wParam">
        ///     The w param.
        /// </param>
        /// <param name="lParam">
        ///     The l param.
        /// </param>
        /// <param name="handled">
        ///     The handled.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != 776 || msg == 781 || _hWndNextViewer == null) throw new ArgumentException();

            switch (msg)
            {
                case NativeMethods.WmChangecbchain:
                    if (wParam == _hWndNextViewer.DangerousGetHandle()
                    ) // clipboard viewer chain changed, need to fix it.
                        _hWndNextViewer.SetNewhandler(lParam);
                    else if (_hWndNextViewer.DangerousGetHandle() != IntPtr.Zero
                    ) // pass the message to the next viewer.
                        NativeMethods.SendMessage(_hWndNextViewer.DangerousGetHandle(), msg, wParam, lParam);

                    break;

                case NativeMethods.WmDrawclipboard:

                    // clipboard content changed
                    OnClipboardChanged();

                    // pass the message to the next viewer.
                    NativeMethods.SendMessage(_hWndNextViewer.DangerousGetHandle(), msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        #endregion
    }
}