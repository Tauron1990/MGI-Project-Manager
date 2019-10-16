using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Services.Extensions;

namespace CalculatorUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var collection = new ServiceCollection();

            collection.AddLogging();
            collection.AddCQRSServices(cofiguration => cofiguration
                                          //.SetUrls(new Uri("http://localhost:81/", UriKind.RelativeOrAbsolute), "CalculatorUI", "")
                                           .SetUrls(new Uri("http://192.168.105.18:81/", UriKind.RelativeOrAbsolute), "CalculatorUI", "")
                                          .AddFrom<App>());

            ServiceProvider = collection.BuildServiceProvider();
        }
    }
}
