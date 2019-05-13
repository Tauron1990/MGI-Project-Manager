using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;

namespace IISServiceManager.Processing
{
    public sealed class InstallEngine : ViewModelBase
    {
        private bool _canInstallNormal;
        private IWebServiceCluster _webServiceCluster;
        private readonly  IisContainer _iisContainer = new IisContainer();

        public IEnumerable<InstallableService> Services { get; private set; }

        public async Task Initialize(IWebServiceCluster serviceCluster)
        {
            _webServiceCluster = serviceCluster;

            List<InstallableService> services = (from webService in await serviceCluster.GetServices() select new InstallableService(webService)).ToList();
            Services = services.AsReadOnly();

            CanInstallNormal = Services.Where(s => s.WebService.ServiceType == ServiceType.Essential)
                .All(s => s.IsInstalled != ServiceStade.NotInstalled);
        }

        public async Task StopService(IWebService service)
        {

        }

        public async Task StartService(IWebService service)
        {

        }

        public async Task InstallService(IWebService service)
        {

        }

        public async Task UpdateSerivce(IWebService service)
        {

        }

        public async Task UpdateAll()
        {

        }

        public async Task UpdateRepo()
        {

        }

        public bool CanInstallNormal
        {
            get => _canInstallNormal;
            set => Set(ref _canInstallNormal, value);
        }
    }
}