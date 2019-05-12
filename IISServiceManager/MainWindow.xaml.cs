using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using WPFLocalizeExtension.Engine;

namespace IISServiceManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SyncfusionLicenseProvider.RegisterLicense("OTk3NjNAMzEzNzJlMzEyZTMwZUt2NDI4Y1JuZEkrWWZlMXRDSmgybWxxNEl3RTJ2NktBdmZZNW9kODl3Zz0=");
            LocalizeDictionary.Instance.Culture = CultureInfo.CurrentUICulture;

            InitializeComponent();
        }
    }
}
