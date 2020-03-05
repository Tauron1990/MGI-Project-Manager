using System.Windows;
using Syncfusion.SfSkinManager;

namespace Tauron.Application.Deployment.AutoUpload.Core.UI
{
    /// <summary>
    ///     Interaktionslogik für InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
            SfSkinManager.SetVisualStyle(this, VisualStyles.Blend);

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = System.Windows.Application.Current.MainWindow;
        }

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
                Cancelbutton.IsEnabled = value;
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