// The file PrgressDialog.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="PrgressDialog.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The simple progress dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;
using Ookii.Dialogs.Wpf;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The simple progress dialog.</summary>
    public sealed class SimpleProgressDialog : IProgressDialog
    {
        private class DialogReporter : IProgress<ActiveProgress>
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DialogReporter" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="DialogReporter" /> Klasse.
            /// </summary>
            /// <param name="dialog">
            ///     The dialog.
            /// </param>
            /// <param name="text">
            ///     The text.
            /// </param>
            public DialogReporter([NotNull] ProgressDialog dialog, [NotNull] string text)
            {
                _dialog = dialog;
                _text = text;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The report.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            public void Report([NotNull] ActiveProgress value)
            {
                _dialog.ReportProgress(
                    (int) value.OverAllProgress,
                    _text,
                    string.Format("{0} : {1}%", value.Message, (int) value.Percent));
            }

            #endregion

            #region Fields

            private readonly ProgressDialog _dialog;

            private readonly string _text;

            #endregion
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleProgressDialog" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SimpleProgressDialog" /> Klasse.
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
        public SimpleProgressDialog([NotNull] string text, [NotNull] string title, [NotNull] IWindow owner, [NotNull] Action<IProgress<ActiveProgress>> worker)
        {
            _owner = owner;
            _worker = worker;
            _dialog = new ProgressDialog
            {
                Text = text,
                ShowTimeRemaining = false,
                WindowTitle = title,
                ShowCancelButton = false
            };
            _dialog.DoWork += DoWork;
            _dialog.RunWorkerCompleted += RunWorkerCompleted;
        }

        #endregion

        #region Public Events

        /// <summary>The completed.</summary>
        public event EventHandler Completed;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the progress bar style.</summary>
        public ProgressStyle ProgressBarStyle
        {
            get => (ProgressStyle) _dialog.ProgressBarStyle;

            set => _dialog.ProgressBarStyle = (ProgressBarStyle) value;
        }

        #endregion

        #region Fields

        private readonly ProgressDialog _dialog;

        private readonly IWindow _owner;

        private readonly Action<IProgress<ActiveProgress>> _worker;

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _dialog.Dispose();
        }

        /// <summary>The start.</summary>
        public void Start()
        {
            ObservableObject.CurrentDispatcher.Invoke(
                () =>
                {
                    if (_owner == null) _dialog.ShowDialog();
                    else
                        _dialog.ShowDialog(
                            (Window)
                            _owner.TranslateForTechnology());
                });
        }

        #endregion

        #region Methods

        private void DoWork([NotNull] object sender, [NotNull] DoWorkEventArgs e)
        {
            _worker(new DialogReporter(_dialog, _dialog.Description));
        }

        private void RunWorkerCompleted([NotNull] object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            Dispose();
            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        #endregion
    }
}