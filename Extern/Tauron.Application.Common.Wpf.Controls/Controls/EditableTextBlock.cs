// The file EditableTextBlock.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="EditableTextBlock.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The editable text block.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The editable text block.</summary>
    [PublicAPI]
    public class EditableTextBlock : TextBlock
    {
        #region Fields

        private EditableTextBlockAdorner _adorner;

        #endregion

        #region Static Fields

        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(
                EditableTextBlock
            ),
            new UIPropertyMetadata
            (false,
                IsInEditModeUpdate));

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength",
            typeof(int),
            typeof(
                EditableTextBlock),
            new UIPropertyMetadata
                (0));

        #endregion

        // Using a DependencyProperty as the backing store for IsInEditMode.  This enables animation, styling, binding, etc...

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is in edit mode.</summary>
        public bool IsInEditMode
        {
            get => (bool) GetValue(IsInEditModeProperty);

            set => SetValue(IsInEditModeProperty, value);
        }

        /// <summary>
        ///     Gets or sets the length of the max.
        /// </summary>
        /// <value>The length of the max.</value>
        public int MaxLength
        {
            get => (int) GetValue(MaxLengthProperty);

            set => SetValue(MaxLengthProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invoked when an unhandled
        ///     <see>
        ///         <cref>E:System.Windows.Input.Mouse.MouseDown</cref>
        ///     </see>
        ///     attached event reaches an element in its route that is derived from this class. Implement this method to add class
        ///     handling for this event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data
        ///     reports details about the mouse button that was pressed and the handled state.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) IsInEditMode = true;
            else if (e.ClickCount == 2) IsInEditMode = true;
        }

        /// <summary>
        ///     Determines whether [is in edit mode update] [the specified obj].
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
        private static void IsInEditModeUpdate([NotNull] DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = obj as EditableTextBlock;
            if (null == textBlock) return;

            // Get the adorner layer of the uielement (here TextBlock)
            var layer = AdornerLayer.GetAdornerLayer(textBlock);

            // If the IsInEditMode set to true means the user has enabled the edit mode then
            // add the adorner to the adorner layer of the TextBlock.
            if (textBlock.IsInEditMode)
            {
                if (null == textBlock._adorner)
                {
                    textBlock._adorner = new EditableTextBlockAdorner(textBlock);

                    // Events wired to exit edit mode when the user presses Enter key or leaves the control.
                    textBlock._adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
                    textBlock._adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
                }

                layer.Add(textBlock._adorner);
            }
            else
            {
                // Remove the adorner from the adorner layer.
                var adorners = layer.GetAdorners(textBlock);
                if (adorners != null)
                    foreach (var adorner in adorners.OfType<EditableTextBlockAdorner>())
                        layer.Remove(adorner);

                // Update the textblock's text binding.
                var expression = textBlock.GetBindingExpression(TextProperty);
                expression?.UpdateTarget();
            }
        }

        /// <summary>
        ///     release the edit mode when user presses enter.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The <see cref="System.Windows.Input.KeyEventArgs" /> instance containing the event data.
        /// </param>
        private void TextBoxKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key == Key.Enter) IsInEditMode = false;
        }

        private void TextBoxLostFocus([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            IsInEditMode = false;
        }

        #endregion
    }
}