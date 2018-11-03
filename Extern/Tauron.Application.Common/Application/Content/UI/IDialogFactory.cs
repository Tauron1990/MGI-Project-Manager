using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    [PublicAPI]
    public enum MsgBoxImage
    {
        None = 0,

        Error = 16,

        Hand = 16,

        Stop = 16,

        Question = 32,

        Exclamation = 48,

        Warning = 48,

        Asterisk = 64,

        Information = 64
    }

    [PublicAPI]
    public enum MsgBoxButton
    {
        Ok = 0,

        OkCancel = 1,

        YesNoCancel = 3,

        YesNo = 4
    }

    [PublicAPI]
    public enum MsgBoxResult
    {
        None = 0,

        Ok = 1,

        Cancel = 2,

        Yes = 6,

        No = 7
    }

    [PublicAPI]
    public interface IDialogFactory
    {
        #region Public Methods and Operators

        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [PublicAPI]
        IProgressDialog CreateProgressDialog([NotNull] string text, [NotNull] string title, [CanBeNull] IWindow owner,
            [NotNull] Action<IProgress<ActiveProgress>> worker);


        [PublicAPI]
        void FormatException([CanBeNull] IWindow owner, [NotNull] Exception exception);


        [NotNull]
        [PublicAPI]
        string GetText([CanBeNull] IWindow owner, [NotNull] string instruction, [CanBeNull] string content, [NotNull] string caption,
            bool allowCancel, [CanBeNull] string defaultValue);


        [PublicAPI]
        MsgBoxResult ShowMessageBox([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
            MsgBoxButton button,
            MsgBoxImage icon, [CanBeNull] Icon custumIcon);


        [NotNull]
        IEnumerable<string> ShowOpenFileDialog([CanBeNull] IWindow owner,
            bool checkFileExists, [NotNull] string defaultExt,
            bool dereferenceLinks, [NotNull] string filter,
            bool multiSelect, [NotNull] string title,
            bool validateNames,
            bool checkPathExists,
            out bool? result);


        [NotNull]
        string ShowOpenFolderDialog([CanBeNull] IWindow owner, [NotNull] string description,
            Environment.SpecialFolder rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result);

        [NotNull]
        string ShowOpenFolderDialog([CanBeNull] IWindow owner, [NotNull] string description,
            string rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result);


        [NotNull]
        string ShowSaveFileDialog([CanBeNull] IWindow owner,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists, [NotNull] string defaultExt,
            bool dereferenceLinks, [NotNull] string filter,
            bool createPrompt,
            bool overwritePrompt, [NotNull] string title, [NotNull] string initialDirectory,
            out bool? result);


        [PublicAPI]
        MsgBoxResult ShowTaskDialog([CanBeNull] IWindow owner, [NotNull] string text, [NotNull] string caption,
            MsgBoxButton button,
            MsgBoxImage icon, [CanBeNull] Icon custumIcon);

        #endregion
    }
}