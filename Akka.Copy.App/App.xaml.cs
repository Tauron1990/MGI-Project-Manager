using System;
using System.IO;
using System.Windows;
using Akka.Actor;
using Akka.Configuration;

namespace Akka.Copy.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnExit(object sender, ExitEventArgs e) 
            => CoreSystem.System.Terminate().Wait(TimeSpan.FromSeconds(5));
    }
}
