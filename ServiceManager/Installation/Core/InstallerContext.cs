using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerContext : IDisposable
    {
        private ZipArchive _packageArchive;

        public InstallerContext(IServiceScope serviceScope, string packagePath)
        {
            ServiceScope = serviceScope;
            PackagePath = packagePath;
        }

        public string PackagePath { get; }

        public string ServiceName { get; set; }

        public IServiceScope ServiceScope { get; }

        public ZipArchive PackageArchive
        {
            get
            {
                if (_packageArchive == null)
                {
                    try
                    {
                        _packageArchive = new ZipArchive(File.Open(PackagePath, FileMode.Open), ZipArchiveMode.Read, false);
                    }
                    catch
                    {
                        return null;
                    }
                }

                return _packageArchive;
            }
        }


        public void Dispose()
            => _packageArchive?.Dispose();

        public RunningService CreateRunningService() 
            => new RunningService(PackagePath, ServiceStade.Ready, ServiceName);
    }
}