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
        private class ViewerSafeHandle : SafeHandleMinusOneIsInvalid
        {
            #region Constructors and Destructors


            public ViewerSafeHandle([NotNull] IWindow current)
                : base(true)
            {
                NativeMethods.AddClipboardFormatListener(current.Handle);
                SetHandle(current.Handle);
            }

            #endregion

            protected override bool ReleaseHandle()
            {
                return NativeMethods.RemoveClipboardFormatListener(DangerousGetHandle());
            }
         }

        /// <summary>The clipboard changed.</summary>
        public event EventHandler ClipboardChanged;


        /// <summary>The _disposed.</summary>
        private bool _disposed;

        /// <summary>The _h wnd next viewer.</summary>
        private ViewerSafeHandle _hWndViewer;

        /// <summary>The _is viewing.</summary>
        private bool _isViewing;

        /// <summary>The _target.</summary>
        [CanBeNull]
        private IWindow _target;

        #region Constructors and Destructors

        public ClipboardViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            if (registerForClose) _target.Closed += TargetClosed;

            if (performInitialization) Initialize();
        }

        ~ClipboardViewer() => Dispose();

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
                _hWndViewer = new ViewerSafeHandle(_target ?? throw new InvalidOperationException("No Window")); // set this window as a viewer
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
            _hWndViewer.Dispose();

            _hWndViewer = null;
            _target?.RemoveHook(WinProc);
            _isViewing = false;
        }

        /// <summary>The on clipboard changed.</summary>
        private void OnClipboardChanged()
        {
            var handler = ClipboardChanged;
            handler?.Invoke(this, EventArgs.Empty);
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
        private void TargetClosed([NotNull] object sender, [NotNull] EventArgs e) => Dispose();


        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != 776 || msg == 781 || _hWndViewer == null) throw new ArgumentException();

            switch (msg)
            {
                case NativeMethods.WM_CLIPBOARDUPDATE:

                    // clipboard content changed
                    OnClipboardChanged();
                    break;
            }

            return IntPtr.Zero;
        }

        #endregion
    }
}