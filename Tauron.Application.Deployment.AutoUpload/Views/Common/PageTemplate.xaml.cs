using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Tauron.Application.Deployment.AutoUpload.Views.Common
{
    /// <summary>
    /// Interaktionslogik für PageTemplate.xaml
    /// </summary>
    [ContentProperty(nameof(ControlContent))]
    public partial class PageTemplate
    {
        public PageTemplate()
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }

        public string? Title
        {
            get => TitleBox.Text;
            set => TitleBox.Text = value;
        }

        public object? ControlContent
        {
            get => ContentControl.Content;
            set => ContentControl.Content = value;
        }

        public object? Status
        {
            get => StatusControl.Content;
            set => StatusControl.Content = value;
        }
    }
}
