using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Services;

namespace ServiceManager.Installation.Core
{
    public class InstallerContext : IDisposable
    {
        public const string ServiceSettingsFileName = "ServiceSettings.json";

        private RunningService _runningService;
        private ZipArchive _packageArchive;

        public InstallerContext(IServiceScope serviceScope, string packagePath)
        {
            ServiceScope = serviceScope;
            PackagePath = packagePath;
        }

        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

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

        public string ExeName { get; set; }

        public string InstalledPath { get; set; }

        public void Dispose()
            => _packageArchive?.Dispose();

        public RunningService CreateRunningService()
            => _runningService ??= new RunningService(InstalledPath, ServiceStade.Ready, ServiceName, ExeName);

        private InstallerContext(RunningService runningService, IServiceScope serviceScope)
        {
            InstalledPath = runningService.InstallationPath;
            ExeName = runningService.Exe;
            ServiceScope = serviceScope;
            ServiceName = runningService.Name;
            _runningService = runningService;
        }

        public static InstallerContext CreateFrom(RunningService runningService, IServiceScope serviceScope) 
            => new InstallerContext(runningService, serviceScope);
    }
}