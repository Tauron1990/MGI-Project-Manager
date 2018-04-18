using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The msg box image.</summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    [PublicAPI]
    public enum MsgBoxImage
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The error.</summary>
        Error = 16,

        /// <summary>The hand.</summary>
        Hand = 16,

        /// <summary>The stop.</summary>
        Stop = 16,

        /// <summary>The question.</summary>
        Question = 32,

        /// <summary>The exclamation.</summary>
        Exclamation = 48,

        /// <summary>The warning.</summary>
        Warning = 48,

        /// <summary>The asterisk.</summary>
        Asterisk = 64,

        /// <summary>The information.</summary>
        Information = 64
    }

    /// <summary>The msg box button.</summary>
    [PublicAPI]
    public enum MsgBoxButton
    {
        /// <summary>The ok.</summary>
        Ok = 0,

        /// <summary>The ok cancel.</summary>
        OkCancel = 1,

        /// <summary>The yes no cancel.</summary>
        YesNoCancel = 3,

        /// <summary>The yes no.</summary>
        YesNo = 4
    }

    /// <summary>The msg box result.</summary>
    [PublicAPI]
    public enum MsgBoxResult
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The ok.</summary>
        Ok = 1,

        /// <summary>The cancel.</summary>
        Cancel = 2,

        /// <summary>The yes.</summary>
        Yes = 6,

        /// <summary>The no.</summary>
        No = 7
    }

    /// <summary>The DialogFactory interface.</summary>
    [PublicAPI]
    public interface IDialogFactory
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create progress dialog.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="worker">
        ///     The worker.
        /// </param>
        /// <returns>
        ///     The <see cref="IProgressDialog" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [PublicAPI]
        IProgressDialog CreateProgressDialog([NotNull] string text, [NotNull] string title, [CanBeNull] IWindow owner, [NotNull] Action<IProgress<ActiveProgress>> worker);

        /// <summary>
        ///     The format exception.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="exception">
        ///     The exception.
        /// </param>
        [PublicAPI]
        void FormatException([CanBeNull] IWindow owner, [NotNull] Exception exception);

        /// <summary>
        ///     The get text.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="instruction">
        ///     The instruction.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="allowCancel">
        ///     The allow cancel.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        [PublicAPI]
        string GetText([CanBeNull] IWindow owner,       [NotNull]   string instruction, [CanBeNull] string content, [NotNull] string caption,
                       bool                allowCancel, [CanBeNull] string defaultValue);

        /// <summary>
        ///     The show message box.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="button">
        ///     The button.
        /// </param>
        /// <param name="icon">
        ///     The icon.
        /// </param>
        /// <param name="custumIcon">
        ///     The custum icon.
        /// </param>
        /// <returns>
        ///     The <see cref="MsgBoxResult" />.
        /// </returns>
        [PublicAPI]
        MsgBoxResult ShowMessageBox([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
                                    MsgBoxButton        button,
                                    MsgBoxImage         icon, [CanBeNull] Icon custumIcon);

        /// <summary>
        ///     The show open file dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="checkFileExists">
        ///     The check file exists.
        /// </param>
        /// <param name="defaultExt">
        ///     The default ext.
        /// </param>
        /// <param name="dereferenceLinks">
        ///     The dereference links.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="multiSelect">
        ///     The multi select.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="validateNames">
        ///     The validate names.
        /// </param>
        /// <param name="checkPathExists">
        ///     The check path exists.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        [NotNull]
        IEnumerable<string> ShowOpenFileDialog([CanBeNull] IWindow owner,
                                               bool                checkFileExists,  [NotNull] string defaultExt,
                                               bool                dereferenceLinks, [NotNull] string filter,
                                               bool                multiSelect,      [NotNull] string title,
                                               bool                validateNames,
                                               bool                checkPathExists,
                                               out bool?           result);

        /// <summary>
        ///     The show open folder dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <param name="rootFolder">
        ///     The root folder.
        /// </param>
        /// <param name="showNewFolderButton">
        ///     The show new folder button.
        /// </param>
        /// <param name="useDescriptionForTitle">
        ///     The use description for title.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        string ShowOpenFolderDialog([CanBeNull] IWindow       owner, [NotNull] string description,
                                    Environment.SpecialFolder rootFolder,
                                    bool                      showNewFolderButton,
                                    bool                      useDescriptionForTitle,
                                    out bool?                 result);

        /// <summary>
        ///     The show save file dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="addExtension">
        ///     The add extension.
        /// </param>
        /// <param name="checkFileExists">
        ///     The check file exists.
        /// </param>
        /// <param name="checkPathExists">
        ///     The check path exists.
        /// </param>
        /// <param name="defaultExt">
        ///     The default ext.
        /// </param>
        /// <param name="dereferenceLinks">
        ///     The dereference links.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="createPrompt">
        ///     The create prompt.
        /// </param>
        /// <param name="overwritePrompt">
        ///     The overwrite prompt.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="initialDirectory">
        ///     The initial directory.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        string ShowSaveFileDialog([CanBeNull] IWindow owner,
                                  bool                addExtension,
                                  bool                checkFileExists,
                                  bool                checkPathExists,  [NotNull] string defaultExt,
                                  bool                dereferenceLinks, [NotNull] string filter,
                                  bool                createPrompt,
                                  bool                overwritePrompt, [NotNull] string title, [NotNull] string initialDirectory,
                                  out bool?           result);

        /// <summary>
        ///     The show task dialog.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="caption">
        ///     The caption.
        /// </param>
        /// <param name="button">
        ///     The button.
        /// </param>
        /// <param name="icon">
        ///     The icon.
        /// </param>
        /// <param name="custumIcon">
        ///     The custum icon.
        /// </param>
        /// <returns>
        ///     The <see cref="MsgBoxResult" />.
        /// </returns>
        [PublicAPI]
        MsgBoxResult ShowTaskDialog([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
                                    MsgBoxButton        button,
                                    MsgBoxImage         icon, [CanBeNull] Icon custumIcon);

        #endregion
    }
}