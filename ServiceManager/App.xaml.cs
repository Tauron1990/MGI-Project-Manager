using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PresentationTheme.Aero;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = AeroTheme.ResourceUri
            });
        }
    }
}
