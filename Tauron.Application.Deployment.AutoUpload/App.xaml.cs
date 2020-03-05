﻿using System;
using System.Windows;
using System.Windows.Input;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Syncfusion.Licensing;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Wpf;
using InternalSplashScreen = Tauron.Application.Deployment.AutoUpload.Core.InternalSplashScreen;

namespace Tauron.Application.Deployment.AutoUpload
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string SyncfusionKey = "MTc0Mjk2QDMxMzcyZTMzMmUzMElNUnVpcGhkMFhTMThkRzcvM2hSMENDc2c2YURtQS95bXhJSzVXaDduUEE9";

        public App()
        {
            SyncfusionLicenseProvider.RegisterLicense(SyncfusionKey);
        }

        public static IServiceProvider ServiceProvider => ((App) Current).ServiceLocator!;

        private IServiceLocator? ServiceLocator { get; set; }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var screen = new InternalSplashScreen();
            screen.Show();

            ServiceLocator = IOCReplacer.Create(serviceCollection =>
            {
                serviceCollection
                    .AddSingleton(
                        sp
                            => new GitHubClient(
                                new ProductHeaderValue("Tauron.Application.Deployment.AutoUpload")));

                serviceCollection.AddSingleton(s => Settings.Create());
                serviceCollection.AddLogging();

                serviceCollection.AddSingleton(Current.Dispatcher);
            });


            CommandBinder.Register(new RoutedUICommand("Weiter", "NextCommand", typeof(App), new InputGestureCollection {new KeyGesture(Key.Enter, ModifierKeys.Control)}));
            MainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            MainWindow?.Show();
            screen.Hide();
        }
    }
}