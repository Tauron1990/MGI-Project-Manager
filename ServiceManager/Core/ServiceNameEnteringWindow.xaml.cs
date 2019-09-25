using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServiceManager.Core
{
    /// <summary>
    /// Interaktionslogik für ServiceNameEnteringWindow.xaml
    /// </summary>
    public partial class ServiceNameEnteringWindow : Window
    {
        public ServiceNameEnteringWindow(ServiceSettings serviceSettings)
        {
            InitializeComponent();

            DataContext = new ServiceNameEnteringWindowModel(serviceSettings);
        }
    }
}
