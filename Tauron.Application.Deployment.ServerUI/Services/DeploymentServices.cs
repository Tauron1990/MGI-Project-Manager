using System;
using Scrutor;
using Tauron.Application.Deployment.Server.Services;

namespace Tauron.Application.Deployment.ServerUI.Services
{
    [ServiceDescriptor]
    public sealed class DeploymentServices
    {
        private readonly Lazy<DownloadService.DownloadServiceClient> _downlod;
        private readonly Lazy<RepositoryService.RepositoryServiceClient> _repository;
        private readonly Lazy<SoftwareService.SoftwareServiceClient> _software;


        public DeploymentServices()
        {
            
        }
    }
}