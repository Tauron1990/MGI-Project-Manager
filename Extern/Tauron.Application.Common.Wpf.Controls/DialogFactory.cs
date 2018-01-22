// The file DialogFactory.cs is part of Tauron.Application.Common.Wpf.Controls.
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
//  along with Tauron.Application.Common.Wpf.Controls If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The dialog factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;
using Tauron.Application.Controls;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>The dialog factory.</summary>
    [Export(typeof(IDialogFactory))]
    public class DialogFactory : IDialogFactory
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
        public IProgressDialog CreateProgressDialog(
            string text,
            string title,
            IWindow owner,
            Action<IProgress<ActiveProgress>> worker)
        {
            return new SimpleProgressDialog(text, title, owner, worker);
        }

        /// <summary>
        ///     The format exception.
        /// </summary>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <param name="exception">
        ///     The exception.
        /// </param>
        public void FormatException(IWindow owner, Exception exception)
        {
            ShowMessageBox(
                owner,
                string.Format("Type: {0} \n {1}", exception.GetType(), exception.Message),
                "Error",
                MsgBoxButton.Ok,
                MsgBoxImage.Error,
                Properties.Resources.Erroricon);
        }

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
        public string GetText(
            IWindow owner,
            string instruction,
            string content,
            string caption,
            bool allowCancel,
            string defaultValue)
        {
            return ObservableObject.CurrentDispatcher.Invoke(
                () =>
                {
                    var realWindow = owner == null ? null : (Window) owner.TranslateForTechnology();
                    var diag = new InputDialog
                    {
                        Owner = realWindow,
                        MainText = instruction,
                        AllowCancel = allowCancel,
                        Title = caption,
                        InstructionText = content,
                        Result = defaultValue
                    };

                    return diag.ShowDialog() == true ? diag.Result : null;
                });
        }


        public MsgBoxResult ShowMessageBox(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            var realWindow = owner == null ? null : (Window) owner.TranslateForTechnology();

            return
                ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                        !TaskDialog.OSSupportsTaskDialogs
                            ? (MsgBoxResult)
                            MessageBox.Show(
                                realWindow,
                                text,
                                caption,
                                (MessageBoxButton) button,
                                (MessageBoxImage) icon)
                            : ShowTaskDialog(owner, text, caption, button, icon,
                                custumIcon));
        }


        public IEnumerable<string> ShowOpenFileDialog(
            IWindow owner,
            bool checkFileExists,
            string defaultExt,
            bool dereferenceLinks,
            string filter,
            bool multiSelect,
            string title,
            bool validateNames,
            bool checkPathExists,
            out bool? result)
        {
            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaOpenFileDialog
                        {
                            CheckFileExists = checkFileExists,
                            DefaultExt = defaultExt,
                            DereferenceLinks =
                                dereferenceLinks,
                            Filter = filter,
                            Multiselect = multiSelect,
                            Title = title,
                            ValidateNames = validateNames,
                            CheckPathExists = checkPathExists
                        };

                        TranslateDefaultExt(dialog);

                        tempresult = owner != null
                            ? dialog.ShowDialog(
                                (Window)
                                owner
                                    .TranslateForTechnology
                                    ())
                            : dialog.ShowDialog();

                        return tempresult == false
                            ? Enumerable.Empty<string>()
                            : dialog.FileNames;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public string ShowOpenFolderDialog(
            IWindow owner,
            string description,
            Environment.SpecialFolder rootFolder,
            bool showNewFolderButton,
            bool useDescriptionForTitle,
            out bool? result)
        {
            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaFolderBrowserDialog
                        {
                            Description = description,
                            RootFolder = rootFolder,
                            ShowNewFolderButton =
                                showNewFolderButton,
                            UseDescriptionForTitle =
                                useDescriptionForTitle
                        };

                        tempresult = owner != null
                            ? dialog.ShowDialog(
                                (Window)
                                owner
                                    .TranslateForTechnology
                                    ())
                            : dialog.ShowDialog();

                        return tempresult == false
                            ? null
                            : dialog.SelectedPath;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public string ShowSaveFileDialog(
            IWindow owner,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists,
            string defaultExt,
            bool dereferenceLinks,
            string filter,
            bool createPrompt,
            bool overwritePrompt,
            string title,
            string initialDirectory,
            out bool? result)
        {
            bool? tempresult = null;

            try
            {
                return ObservableObject.CurrentDispatcher.Invoke(
                    () =>
                    {
                        var dialog = new VistaSaveFileDialog
                        {
                            AddExtension = addExtension,
                            CheckFileExists = checkFileExists,
                            DefaultExt = defaultExt,
                            DereferenceLinks =
                                dereferenceLinks,
                            Filter = filter,
                            Title = title,
                            CheckPathExists = checkPathExists,
                            CreatePrompt = createPrompt,
                            OverwritePrompt = overwritePrompt,
                            InitialDirectory =
                                initialDirectory
                        };

                        TranslateDefaultExt(dialog);

                        tempresult = owner != null
                            ? dialog.ShowDialog(
                                (Window)
                                owner
                                    .TranslateForTechnology
                                    ())
                            : dialog.ShowDialog();

                        return tempresult == false ? null : dialog.FileName;
                    });
            }
            finally
            {
                result = tempresult;
            }
        }

        public MsgBoxResult ShowTaskDialog(
            IWindow owner,
            string text,
            string caption,
            MsgBoxButton button,
            MsgBoxImage icon,
            Icon custumIcon)
        {
            return ObservableObject.CurrentDispatcher.Invoke(
                () =>
                {
                    var dialog = new TaskDialog
                    {
                        CenterParent = true,
                        Content = text,
                        ExpandFooterArea = false,
                        ExpandedByDefault = false,
                        MinimizeBox = false,
                        ProgressBarStyle =
                            ProgressBarStyle.None,
                        WindowIcon = custumIcon,
                        WindowTitle = caption,
                        MainInstruction = caption,
                        MainIcon = TranslateIcon(icon)
                    };

                    TranslateButtons(button, dialog.Buttons);
                    var clickedButton =
                        dialog.ShowDialog(owner != null
                            ? (Window)
                            owner
                                .TranslateForTechnology
                                ()
                            : null);

                    switch (clickedButton.ButtonType)
                    {
                        case ButtonType.Ok:
                            return MsgBoxResult.Ok;
                        case ButtonType.Yes:
                            return MsgBoxResult.Yes;
                        case ButtonType.No:
                            return MsgBoxResult.No;
                        case ButtonType.Cancel:
                            return MsgBoxResult.Cancel;
                        case ButtonType.Close:
                            return MsgBoxResult.Cancel;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        #endregion

        #region Methods

        private void TranslateDefaultExt([NotNull] VistaFileDialog dialog)
        {
            if (string.IsNullOrWhiteSpace(dialog.DefaultExt)) return;

            var ext = "*." + dialog.DefaultExt;
            var filter = dialog.Filter;
            var filters = filter.Split('|');
            for (var i = 1; i < filters.Length; i += 2) if (filters[i] == ext) dialog.FilterIndex = 1 + (i - 1) / 2;
        }

        private void TranslateButtons(MsgBoxButton button, [NotNull] TaskDialogItemCollection<TaskDialogButton> buttons)
        {
            switch (button)
            {
                case MsgBoxButton.OkCancel:
                    buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    buttons.Add(new TaskDialogButton(ButtonType.Cancel));
                    break;
                case MsgBoxButton.Ok:
                    buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    break;
                case MsgBoxButton.YesNoCancel:
                    buttons.Add(new TaskDialogButton(ButtonType.Yes));
                    buttons.Add(new TaskDialogButton(ButtonType.No));
                    buttons.Add(new TaskDialogButton(ButtonType.Cancel));
                    break;
                case MsgBoxButton.YesNo:
                    buttons.Add(new TaskDialogButton(ButtonType.Yes));
                    buttons.Add(new TaskDialogButton(ButtonType.No));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("button");
            }
        }

        private TaskDialogIcon TranslateIcon(MsgBoxImage icon)
        {
            switch (icon)
            {
                case MsgBoxImage.None:
                    return TaskDialogIcon.Custom;
                case MsgBoxImage.Error:
                    return TaskDialogIcon.Error;
                case MsgBoxImage.Question:
                    return TaskDialogIcon.Shield;
                case MsgBoxImage.Warning:
                    return TaskDialogIcon.Warning;
                case MsgBoxImage.Information:
                    return TaskDialogIcon.Information;
                default:
                    throw new ArgumentOutOfRangeException("icon");
            }
        }

        #endregion
    }
}