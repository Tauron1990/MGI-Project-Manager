// The file nativemethods.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#endregion

namespace Tauron.Interop
{
    /// <summary>The native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>The copy progress routine.</summary>
        /// <param name="totalFileSize">The total file size.</param>
        /// <param name="totalBytesTransferred">The total bytes transferred.</param>
        /// <param name="streamSize">The stream size.</param>
        /// <param name="streamBytesTransferred">The stream bytes transferred.</param>
        /// <param name="dwStreamNumber">The dw stream number.</param>
        /// <param name="dwCallbackReason">The dw callback reason.</param>
        /// <param name="hSourceFile">The h source file.</param>
        /// <param name="hDestinationFile">The h destination file.</param>
        /// <param name="lpData">The lp data.</param>
        /// <returns>The CopyProgressResult.</returns>
        public delegate CopyProgressResult CopyProgressRoutine(
            long                       totalFileSize,
            long                       totalBytesTransferred,
            long                       streamSize,
            long                       streamBytesTransferred,
            uint                       dwStreamNumber,
            CopyProgressCallbackReason dwCallbackReason,
            IntPtr                     hSourceFile,
            IntPtr                     hDestinationFile,
            IntPtr                     lpData);

        /// <summary>The copy progress callback reason.</summary>
        public enum CopyProgressCallbackReason : uint
        {
            /// <summary>The callbac k_ chun k_ finished.</summary>
            CALLBACK_CHUNK_FINISHED = 0x00000000,

            /// <summary>The callbac k_ strea m_ switch.</summary>
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        /// <summary>The copy progress result.</summary>
        public enum CopyProgressResult : uint
        {
            /// <summary>The progres s_ continue.</summary>
            PROGRESS_CONTINUE = 0,

            /// <summary>The progres s_ cancel.</summary>
            PROGRESS_CANCEL = 1,

            /// <summary>The progres s_ stop.</summary>
            PROGRESS_STOP = 2,

            /// <summary>The progres s_ quiet.</summary>
            PROGRESS_QUIET = 3
        }

        /// <summary>The copy file flags.</summary>
        [Flags]
        internal enum CopyFileFlags : uint
        {
            /// <summary>The cop y_ fil e_ fai l_ i f_ exists.</summary>
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,

            /// <summary>The cop y_ fil e_ n o_ buffering.</summary>
            COPY_FILE_NO_BUFFERING = 0x00001000,

            /// <summary>The cop y_ fil e_ restartable.</summary>
            COPY_FILE_RESTARTABLE = 0x00000002,

            /// <summary>The cop y_ fil e_ ope n_ sourc e_ fo r_ write.</summary>
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,

            /// <summary>The cop y_ fil e_ allo w_ decrypte d_ destination.</summary>
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008
        }

        /// <summary>The wm drawclipboard.</summary>
        internal const int WmDrawclipboard = 0x0308;

        /// <summary>The wm changecbchain.</summary>
        internal const int WmChangecbchain = 0x030D;

        /// <summary>
        ///     The set clipboard viewer.
        /// </summary>
        /// <param name="hWndNewViewer">
        ///     The h wnd new viewer.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        /// <summary>
        ///     The change clipboard chain.
        /// </summary>
        /// <param name="hWndRemove">
        ///     The h wnd remove.
        /// </param>
        /// <param name="hWndNewNext">
        ///     The h wnd new next.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        /// <summary>
        ///     The send message.
        /// </summary>
        /// <param name="hWnd">
        ///     The h wnd.
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
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///     The _ command line to argv w.
        /// </summary>
        /// <param name="cmdLine">
        ///     The cmd line.
        /// </param>
        /// <param name="numArgs">
        ///     The num args.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        [DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
        private static extern IntPtr _CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string cmdLine,
            out                               int    numArgs);

        /// <summary>
        ///     The _ local free.
        /// </summary>
        /// <param name="hMem">
        ///     The h mem.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        [DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
        private static extern IntPtr _LocalFree(IntPtr hMem);

        /// <summary>
        ///     The command line to argv w.
        /// </summary>
        /// <param name="cmdLine">
        ///     The cmd line.
        /// </param>
        /// <returns>
        ///     The <see cref="string[]" />.
        /// </returns>
        /// <exception cref="Win32Exception">
        /// </exception>
        public static string[] CommandLineToArgvW(string cmdLine)
        {
            var argv = IntPtr.Zero;
            try
            {
                int numArgs;

                argv = _CommandLineToArgvW(cmdLine, out numArgs);
                if (argv == IntPtr.Zero) throw new Win32Exception();

                var result = new string[numArgs];

                for (var i = 0; i < numArgs; i++)
                {
                    var currArg = Marshal.ReadIntPtr(argv, i * Marshal.SizeOf(typeof(IntPtr)));
                    result[i] = Marshal.PtrToStringUni(currArg);
                }

                return result;
            }
            finally
            {
                _LocalFree(argv);

                // Otherwise LocalFree failed.
                // Assert.AreEqual(IntPtr.Zero, p);
            }
        }

        /// <summary>
        ///     The copy file ex.
        /// </summary>
        /// <param name="lpExistingFileName">
        ///     The lp existing file name.
        /// </param>
        /// <param name="lpNewFileName">
        ///     The lp new file name.
        /// </param>
        /// <param name="lpProgressRoutine">
        ///     The lp progress routine.
        /// </param>
        /// <param name="lpData">
        ///     The lp data.
        /// </param>
        /// <param name="pbCancel">
        ///     The pb cancel.
        /// </param>
        /// <param name="dwCopyFlags">
        ///     The dw copy flags.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false,
            ThrowOnUnmappableChar               = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CopyFileEx(
            string              lpExistingFileName,
            string              lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr              lpData,
            ref int             pbCancel,
            CopyFileFlags       dwCopyFlags);

        /// <summary>
        ///     The sh get known folder path.
        /// </summary>
        /// <param name="rfid">
        ///     The rfid.
        /// </param>
        /// <param name="dwFlags">
        ///     The dw flags.
        /// </param>
        /// <param name="hToken">
        ///     The h token.
        /// </param>
        /// <param name="pszPath">
        ///     The psz path.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        [DllImport("shell32.dll")]
        internal static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint                                     dwFlags,
            IntPtr                                   hToken,
            out IntPtr                               pszPath);

        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        public const int WM_CLIPBOARDUPDATE = 0x031D;

        /// <summary>
        /// To find message-only windows, specify HWND_MESSAGE in the hwndParent parameter of the FindWindowEx function.
        /// </summary>
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);
    }
}