using System.Collections.Generic;
using GalaSoft.MvvmLight;
using IISServiceManager.Contratcs;

namespace IISServiceManager.Processing
{
    public sealed class InstallEngine : ViewModelBase
    {
        private bool _canInstallNormal;

        public IEnumerable<InstallableService> Services { get; }

        public void Initialize(IWebServiceCluster serviceCluster)
        {

        }

        public bool CanInstallNormal
        {
            get => _canInstallNormal;
            set => Set(ref _canInstallNormal, value);
        }
    }
}