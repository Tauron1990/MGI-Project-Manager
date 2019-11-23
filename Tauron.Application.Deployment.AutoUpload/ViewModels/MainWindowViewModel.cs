using Catel.Fody;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor(typeof(MainWindowViewModel), ServiceLifetime.Singleton)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        [NoWeaving]
        public BusyService BusyService { get; }

        public MainWindowViewModel(IPleaseWaitService pleaseWaitService)
        {
            Title = "Auto Versionen und Uploads";
            BusyService = (BusyService) pleaseWaitService;
        }

    }
}