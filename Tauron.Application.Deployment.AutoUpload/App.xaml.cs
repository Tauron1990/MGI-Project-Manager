using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using Catel;
using Catel.IoC;
using Catel.Runtime.Serialization;
using Catel.Runtime.Serialization.Json;
using Catel.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Syncfusion.SfSkinManager;
using Tauron.Application.Deployment.AutoUpload.Core;

namespace Tauron.Application.Deployment.AutoUpload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {


        public const string SyncfusionKey = "MTc0Mjk2QDMxMzcyZTMzMmUzMElNUnVpcGhkMFhTMThkRzcvM2hSMENDc2c2YURtQS95bXhJSzVXaDduUEE9";

        public static IServiceProvider ServiceProvider => ((App) Current).ServiceLocator!;

        private IServiceLocator? ServiceLocator { get; set; }

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionKey);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var screen = new InternalSplashScreen();
            screen.Show();

            ServiceLocator = IOCReplacer.Create(serviceCollection =>
                                                {

                                                    serviceCollection.Scan(
                                                        ts =>
                                                            ts.FromApplicationDependencies()
                                                               .AddClasses(c => c.WithAttribute<ControlAttribute>())
                                                               .As<FrameworkElement>().UsingRegistrationStrategy(new ControlRegistrar())
                                                               .AddClasses().UsingAttributes());

                                                    serviceCollection.AddLogging();
                                                    serviceCollection.AddSingleton(Current.Dispatcher);
                                                });


            SfSkinManager.ApplyStylesOnApplication = true;
            MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            MainWindow?.Show();
            screen.Hide();
        }
    }
}
