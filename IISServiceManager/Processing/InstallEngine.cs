using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;
using Microsoft.Web.Administration;

namespace IISServiceManager.Processing
{
    public sealed class InstallEngine : ViewModelBase
    {
        private bool _canInstallNormal;
        private IWebServiceCluster _webServiceCluster;
        private string _repoLocation;
        private bool _updateNeed;

        private readonly  IisContainer _iisContainer = new IisContainer();

        public IEnumerable<InstallableService> Services { get; private set; }

        public async Task Initialize(IWebServiceCluster serviceCluster)
        {
            _webServiceCluster = serviceCluster;
            _repoLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repos", serviceCluster.Id);

            List<InstallableService> services = (from webService in await serviceCluster.GetServices() select new InstallableService(webService)).ToList();
            Services = services.AsReadOnly();

            foreach (var installableService in services)
            {
                var site = await _iisContainer.FindSite(installableService.WebService.Id);
                if (site == null)
                    installableService.ServiceStade = ServiceStade.NotInstalled;
                else
                {
                    switch (site.State)
                    {
                        case ObjectState.Starting:
                        case ObjectState.Started:
                            installableService.ServiceStade = ServiceStade.Running;
                            break;
                        case ObjectState.Stopping:
                        case ObjectState.Stopped:
                        case ObjectState.Unknown:
                            installableService.ServiceStade = ServiceStade.Stopped;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            CanInstallNormal = Services.Where(s => s.WebService.ServiceType == ServiceType.Essential)
                .All(s => s.ServiceStade != ServiceStade.NotInstalled);
        }

        public async Task StartService(IWebService service, ILog log)
        {
            log.WriteLine($"Try Starting WebService... {service.Id}");
            var site = await _iisContainer.FindSite(service.Id);
            if (site == null)
            {
                log.WriteLine("\t WebService not Found");
                log.AutoClose = false;
                return;
            }

            var state = await _iisContainer.StartSite(site);
            log.WriteLine($"\tStarting Compled: {state}");
        }

        public async Task StopService(IWebService service, ILog log)
        {
            log.WriteLine($"Try Stop WebService... {service.Id}");
            var site = await _iisContainer.FindSite(service.Id);
            if (site == null)
            {
                log.WriteLine("\t WebSerivce not Found");
                log.AutoClose = false;
                return;
            }

            var state = await _iisContainer.StopSite(site);
            log.WriteLine($"\tStopping Compled: {state}");
        }

        private async Task<(string path, bool Ok)> BuildSolution(IWebService service, ILog log)
        {
            string targetPath =
                Path.Combine(Properties.Settings.Default.WebsitePath, _webServiceCluster.Id, service.Id);

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            var ok = await _webServiceCluster.Build(_repoLocation, targetPath, service, log);

            return (targetPath, ok);
        }

        public async Task InstallService(IWebService service, ILog log)
        {
            var site = await _iisContainer.FindSite(service.Id);
            if (site != null)
            {
                log.AutoClose = false;
                log.WriteLine("Web Service already Exist");
            }

            var (path, ok) = await BuildSolution(service, log);

            if (!ok)
            {
                log.WriteLine("Build Failed");
                return;
            }

            var state = _iisContainer.CreateSite(path, service, _webServiceCluster, log);

            log.WriteLine($"Status: {state}");
        }

        public async Task UpdateSerivce(IWebService service, ILog log)
        {

        }

        public async Task UpdateAll(ILog log)
        {

        }

        public async Task UpdateRepo(ILog log)
        {

        }

        public async Task Unitstall(IWebService webService, ILog log)
        {

        }

        public bool CanInstallNormal
        {
            get => _canInstallNormal;
            set => Set(ref _canInstallNormal, value);
        }


    }
}