#region

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The window hook.</summary>
    /// <param name="hwnd">The hwnd.</param>
    /// <param name="msg">The msg.</param>
    /// <param name="wParam">The w param.</param>
    /// <param name="lParam">The l param.</param>
    /// <param name="handled">The handled.</param>
    public delegate IntPtr WindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

    [PublicAPI]
    public interface IWindow
    {
        [CanBeNull] object Result { get; }

        #region Public Events

        /// <summary>The closed.</summary>
        event EventHandler Closed;

        #endregion

        void Focus();

        void Hide();

        #region Public Properties

        /// <summary>Gets or sets the title.</summary>
        [NotNull]
        string Title { get; set; }

        IntPtr Handle { get; }

        bool? DialogResult { set; get; }

        bool? IsVisible { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        void AddHook([NotNull] WindowHook winProc);

        /// <summary>The close.</summary>
        void Close();

        /// <summary>
        ///     The remove hook.
        /// </summary>
        /// <param name="winProc">
        ///     The win proc.
        /// </param>
        void RemoveHook([NotNull] WindowHook winProc);

        /// <summary>The show.</summary>
        void Show();

        [NotNull]
        Task ShowDialogAsync([CanBeNull] IWindow window);

        /// <summary>The translate for technology.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        object TranslateForTechnology();

        #endregion
    }
}