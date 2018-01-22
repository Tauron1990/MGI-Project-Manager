#region

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Tauron.Interop;

#endregion

namespace Tauron
{
    /// <summary>
    ///     PInvoke wrapper for CopyEx
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/aa363852.aspx.
    /// </summary>
    [PublicAPI]
    public class XCopy
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="XCopy" /> class from being created.
        ///     Verhindert, dass eine Standardinstanz der <see cref="XCopy" /> Klasse erstellt wird.
        ///     Prevents a default instance of the <see cref="XCopy" /> class from being created.
        /// </summary>
        private XCopy()
        {
            _isCancelled = 0;
        }

        #endregion

        #region Events

        /// <summary>The progress changed.</summary>
        private event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        #endregion

        #region Fields

        /// <summary>The _destination.</summary>
        private string _destination;

        /// <summary>The _file percent completed.</summary>
        private int _filePercentCompleted;

        /// <summary>The _is cancelled.</summary>
        private int _isCancelled;

        /// <summary>The _source.</summary>
        private string _source;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The copy.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        /// <param name="overwrite">
        ///     The overwrite.
        /// </param>
        /// <param name="nobuffering">
        ///     The nobuffering.
        /// </param>
        public static void Copy([NotNull] string source, [NotNull] string destination, bool overwrite, bool nobuffering)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            new XCopy().CopyInternal(source, destination, overwrite, nobuffering, null);
        }

        /// <summary>
        ///     The copy.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        /// <param name="overwrite">
        ///     The overwrite.
        /// </param>
        /// <param name="nobuffering">
        ///     The nobuffering.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        public static void Copy([NotNull] string source, [NotNull] string destination,
            bool overwrite,
            bool nobuffering, [CanBeNull] EventHandler<ProgressChangedEventArgs> handler)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentException("Value cannot be null or empty.", nameof(source));
            if (string.IsNullOrEmpty(destination)) throw new ArgumentException("Value cannot be null or empty.", nameof(destination));

            new XCopy().CopyInternal(source, destination, overwrite, nobuffering, handler);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The copy internal.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        /// <param name="overwrite">
        ///     The overwrite.
        /// </param>
        /// <param name="nobuffering">
        ///     The nobuffering.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <exception cref="Win32Exception">
        /// </exception>
        private void CopyInternal([NotNull] string source, [NotNull] string destination,
            bool overwrite,
            bool nobuffering, [CanBeNull] EventHandler<ProgressChangedEventArgs> handler)
        {
            if (string.IsNullOrEmpty(source)) throw new ArgumentException("Value cannot be null or empty.", nameof(source));
            if (string.IsNullOrEmpty(destination)) throw new ArgumentException("Value cannot be null or empty.", nameof(destination));
            try
            {
                var copyFileFlags = NativeMethods.CopyFileFlags.COPY_FILE_RESTARTABLE;
                if (!overwrite) copyFileFlags |= NativeMethods.CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;

                if (nobuffering) copyFileFlags |= NativeMethods.CopyFileFlags.COPY_FILE_NO_BUFFERING;

                _source = source.GetFullPath();
                _destination = destination.GetFullPath();

                if (handler != null) ProgressChanged += handler;

                NativeMethods.CopyProgressRoutine callback = CopyProgressHandler;

                var result = NativeMethods.CopyFileEx(
                    _source,
                    _destination,
                    callback,
                    IntPtr.Zero,
                    ref _isCancelled,
                    copyFileFlags);
                if (!result) throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception)
            {
                if (handler != null) ProgressChanged -= handler;

                throw;
            }
        }

        /// <summary>
        ///     The copy progress handler.
        /// </summary>
        /// <param name="total">
        ///     The total.
        /// </param>
        /// <param name="transferred">
        ///     The transferred.
        /// </param>
        /// <param name="streamSize">
        ///     The stream size.
        /// </param>
        /// <param name="streamByteTrans">
        ///     The stream byte trans.
        /// </param>
        /// <param name="dwStreamNumber">
        ///     The dw stream number.
        /// </param>
        /// <param name="reason">
        ///     The reason.
        /// </param>
        /// <param name="hSourceFile">
        ///     The h source file.
        /// </param>
        /// <param name="hDestinationFile">
        ///     The h destination file.
        /// </param>
        /// <param name="lpData">
        ///     The lp data.
        /// </param>
        /// <returns>
        ///     The <see cref="Interop.NativeMethods.CopyProgressResult" />.
        /// </returns>
        private NativeMethods.CopyProgressResult CopyProgressHandler(
            long total,
            long transferred,
            long streamSize,
            long streamByteTrans,
            uint dwStreamNumber,
            NativeMethods.CopyProgressCallbackReason reason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData)
        {
            if (reason == NativeMethods.CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED) OnProgressChanged(transferred / (double) total * 100.0);

            return NativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
        }

        /// <summary>
        ///     The on progress changed.
        /// </summary>
        /// <param name="percent">
        ///     The percent.
        /// </param>
        private void OnProgressChanged(double percent)
        {
            // only raise an event when progress has changed
            if ((int) percent <= _filePercentCompleted) return;

            _filePercentCompleted = (int) percent;

            var handler = ProgressChanged;
            if (handler != null) handler(this, new ProgressChangedEventArgs(_filePercentCompleted, null));
        }

        #endregion
    }
}