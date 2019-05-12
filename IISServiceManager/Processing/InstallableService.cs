using System.Drawing;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;

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

        public InstallableService(IWebService service)
        {
            WebService = service;
        }
    }
}