using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;
using LibGit2Sharp;
using Microsoft.Web.Administration;

namespace IISServiceManager.Processing
{
    public sealed class InstallEngine : ViewModelBase
    {
        private readonly string _backupPath;

        private bool _canInstallNormal;
        private IWebServiceCluster _webServiceCluster;
        private string _repoLocation;
        private bool _updateNeed;

        private readonly  IisContainer _iisContainer = new IisContainer();

        public IEnumerable<InstallableService> Services { get; private set; }

        public InstallEngine() 
            => _backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");

        public async Task Initialize(IWebServiceCluster serviceCluster)
        {
            _webServiceCluster = serviceCluster;
            _repoLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repos", serviceCluster.Id);
            _repoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repos");

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
            var targetPath = GetBuildPath(service);

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

            Directory.CreateDirectory(GetBuildPath(service));

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
            if (_updateNeed)
            {
                log.WriteLine("No Update Needed...");
                return;
            }

            await StopService(service, log);
            
            _ = await TryUpdateBuild(service, log);

            await StartService(service, log);
        }

        private string GetBuildPath(IWebService webService) 
            => Path.Combine(Properties.Settings.Default.WebsitePath, _webServiceCluster.Id, webService.Id);

        private async Task<(bool OK, string Backup)> TryUpdateBuild(IWebService service, ILog log)
        {
            log.WriteLine("Create backup...");
            string binaryPath = GetBuildPath(service);
            string backupPath = Path.Combine(_backupPath, service.Id + ".zip");

            ZipFile.CreateFromDirectory(binaryPath, backupPath);

            log.WriteLine("Try Build...");
            try
            {
                Directory.Delete(binaryPath, true);
                var (_, ok) = await BuildSolution(service, log);

                if (ok)
                {
                    log.WriteLine("Build Compled...");
                    return (true, backupPath);
                }
            }
            catch (Exception e)
            {
                log.AutoClose = false;
                log.WriteLine(e.ToStringDemystified());
            }

            log.WriteLine("Build Failed... Revert Bachup");
            Directory.Delete(binaryPath, true);
            Directory.CreateDirectory(binaryPath);

            ZipFile.ExtractToDirectory(binaryPath, backupPath);

            return (false, backupPath);
        }

        public async Task UpdateAll(ILog log)
        {
            if (!_updateNeed)
            {
                log.WriteLine("Nod Update Need...");
            }

            log.WriteLine("Start Update All...");

            var services = new List<(string BackupPath, IWebService Services)>();

            bool failed = false;

            foreach (var service in Services)
            {
                if(failed) break;

                try
                {
                    var (ok, backup) = await TryUpdateBuild(service.WebService, log);

                    if (!ok)
                    {
                        failed = true;
                        break;
                    }

                    services.Add((backup, service.WebService));
                }
                catch (Exception e)
                {
                    failed = true;
                    log.AutoClose = false;
                    log.WriteLine(e.ToStringDemystified());
                }
            }

            if (failed)
            {
                log.WriteLine("Updates Failed");

                foreach (var apply in services)
                {
                    try
                    {
                        log.WriteLine("Revert updates");

                        string binaryPath = GetBuildPath(apply.Services);

                        Directory.Delete(binaryPath, true);
                        Directory.CreateDirectory(binaryPath);

                        ZipFile.ExtractToDirectory(binaryPath, apply.BackupPath);
                    }
                    catch (Exception e)
                    {
                        log.WriteLine(e.ToStringDemystified());
                        log.WriteLine($"Revert Failed for {apply.Services.Id}");
                        log.WriteLine("Plese Manul Install Old Version!");
                    }
                }

                return;
            }

            log.WriteLine("Update Compled...");
        }

        public async Task UpdateRepo(ILog log)
        {
            if (Repository.IsValid(_repoLocation))
            {
                var repo = new Repository(_backupPath);

                PullOptions
            }
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