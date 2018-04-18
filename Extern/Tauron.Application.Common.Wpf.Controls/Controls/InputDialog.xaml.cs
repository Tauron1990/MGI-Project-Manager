// The file InputDialog.xaml.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="InputDialog.xaml.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Interaktionslogik für InputDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>
    ///     Interaktionslogik für InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputDialog" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InputDialog" /> Klasse.
        /// </summary>
        public InputDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether allow cancel.</summary>
        public bool AllowCancel
        {
            get => Cancelbutton.IsEnabled;

            set
            {
                Cancelbutton.IsEnabled  = value;
                Cancelbutton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>Gets or sets the instruction text.</summary>
        public string InstructionText
        {
            get => InstructionTextBlock.Text;

            set => InstructionTextBlock.Text = value;
        }

        /// <summary>Gets or sets the main text.</summary>
        public string MainText
        {
            get => MainTextBlock.Text;

            set => MainTextBlock.Text = value;
        }

        /// <summary>Gets or sets the result.</summary>
        public string Result
        {
            get => InputText.Text;

            set => InputText.Text = value;
        }

        #endregion
    }
}