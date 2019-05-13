using System.Drawing;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;
using IISServiceManager.Core;

namespace IISServiceManager.Processing
{
    public class InstallableService : ViewModelBase
    {
        private ServiceStade _isInstalled;

        public ServiceStade IsInstalled
        {
            get => _isInstalled;
            set => Set(ref _isInstalled, value);
        }

        public IWebService WebService { get; }

        public AsyncCommand CommonWebServiceClick { get; set; }

        public AsyncCommand InstallCommand { get; set; }

        public AsyncCommand UnInstallCommand { get; set; }

        public AsyncCommand UpdateCommand { get; set; }

        public InstallableService(IWebService service)
        {
            WebService = service;
        }
    }
}