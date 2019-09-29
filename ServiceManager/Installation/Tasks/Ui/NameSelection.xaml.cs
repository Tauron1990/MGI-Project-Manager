using System.Windows;
using System.Windows.Controls;

namespace ServiceManager.Installation.Tasks.Ui
{
    /// <summary>
    ///     Interaktionslogik für NameSelection.xaml
    /// </summary>
    public partial class NameSelection : UserControl
    {
        private readonly NameSelectionModel _model;

        public NameSelection(NameSelectionModel model)
        {
            InitializeComponent();

            _model = model;
            DataContext = model;
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e) => _model.Click();
    }
}