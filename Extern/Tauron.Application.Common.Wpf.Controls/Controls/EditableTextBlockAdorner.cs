// The file EditableTextBlockAdorner.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="EditableTextBlockAdorner.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Adorner class which shows textbox over the text block when the Edit mode is on.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>
    ///     Adorner class which shows textbox over the text block when the Edit mode is on.
    /// </summary>
    public class EditableTextBlockAdorner : Adorner
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EditableTextBlockAdorner" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="EditableTextBlockAdorner" /> Klasse.
        /// </summary>
        /// <param name="adornedElement">
        ///     The adorned element.
        /// </param>
        public EditableTextBlockAdorner([NotNull] EditableTextBlock adornedElement)
            : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            _textBox = new TextBox();
            _textBlock = adornedElement;

            var binding = new Binding("Text") {Source = adornedElement};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.AcceptsReturn = true;
            _textBox.MaxLength = adornedElement.MaxLength;
            _textBox.KeyUp += TextBoxKeyUpEventHandler;

            _collection.Add(_textBox);
        }

        #endregion

        #region Properties

        /// <summary>Gets the visual children count.</summary>
        protected override int VisualChildrenCount => _collection.Count;

        #endregion

        #region Fields

        private readonly VisualCollection _collection;

        private readonly TextBlock _textBlock;

        private readonly TextBox _textBox;

        #endregion

        #region Public Events

        /// <summary>The text box key up.</summary>
        public event KeyEventHandler TextBoxKeyUp
        {
            add => _textBox.KeyUp += value;

            remove => _textBox.KeyUp -= value;
        }

        /// <summary>The text box lost focus.</summary>
        public event RoutedEventHandler TextBoxLostFocus
        {
            add => _textBox.LostFocus += value;

            remove => _textBox.LostFocus -= value;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The arrange override.
        /// </summary>
        /// <param name="finalSize">
        ///     The final size.
        /// </param>
        /// <returns>
        ///     The <see cref="Size" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBox.Arrange(
                new Rect(0, 0, _textBlock.DesiredSize.Width + 50, _textBlock.DesiredSize.Height * 1.5));
            _textBox.Focus();
            return finalSize;
        }

        /// <summary>
        ///     The get visual child.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="Visual" />.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return _collection[index];
        }

        /// <summary>
        ///     The on render.
        /// </summary>
        /// <param name="drawingContext">
        ///     The drawing context.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(
                null,
                new Pen {Brush = Brushes.Gold, Thickness = 2},
                new Rect(0, 0, _textBlock.DesiredSize.Width + 50,
                    _textBlock.DesiredSize.Height * 1.5));
        }

        private void TextBoxKeyUpEventHandler([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            _textBox.Text = _textBox.Text.Replace("\r\n", string.Empty);
            var expression = _textBox.GetBindingExpression(TextBox.TextProperty);
            if (null != expression) expression.UpdateSource();
        }

        #endregion
    }
}