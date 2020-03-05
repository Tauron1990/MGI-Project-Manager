using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Tauron.Application.Deployment.AutoUpload.Views.Common
{
    /// <summary>
    ///     Interaktionslogik für PageTemplate.xaml
    /// </summary>
    [ContentProperty(nameof(ControlContent))]
    public partial class PageTemplate
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(PageTemplate));
        public static readonly DependencyProperty ControlContentProperty = DependencyProperty.Register(nameof(ControlContent), typeof(object), typeof(PageTemplate));
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(object), typeof(PageTemplate));

        public PageTemplate()
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }

        public string? Title
        {
            get => GetValue(TitleProperty) as string;
            set => SetValue(TitleProperty, value);
        }

        public object? ControlContent
        {
            get => GetValue(ControlContentProperty);
            set => SetValue(ControlContentProperty, value);
        }

        public object? Status
        {
            get => GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }
    }
}