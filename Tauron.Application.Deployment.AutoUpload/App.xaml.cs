﻿using System;
using System.Windows;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Syncfusion.SfSkinManager;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Core;

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
                                                    serviceCollection.AddSingleton(sp => new GitHubClient(new ProductHeaderValue("Tauron.Application.Deployment.AutoUpload")));
                                                    serviceCollection.AddSingleton(s => Settings.Create());
                                                    serviceCollection.Scan(
                                                        ts =>
                                                            ts.FromApplicationDependencies()
                                                               .AddClasses(c => c.WithAttribute<ControlAttribute>())
                                                               .As<FrameworkElement>().UsingRegistrationStrategy(new ControlRegistrar())
                                                               .AddClasses().UsingAttributes());

                                                    serviceCollection.AddLogging();
                                                    serviceCollection.AddSingleton(Current.Dispatcher);
                                                });


            MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            MainWindow?.Show();
            screen.Hide();
        }
    }
}