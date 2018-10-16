using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    [PublicAPI]
    public class LabeledTextbox : TextBox
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledTextbox),
                new PropertyMetadata(null));

        static LabeledTextbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabeledTextbox),
                new FrameworkPropertyMetadata(typeof(LabeledTextbox)));
        }

        [CanBeNull]
        public string LabelText
        {
            get => (string) GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }
    }
}