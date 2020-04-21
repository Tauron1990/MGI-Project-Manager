using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Tauron.Application.Files.VirtualFiles;
using Tauron.Application.Files.VirtualFiles.InMemory;
using Tauron.Application.Files.VirtualFiles.InMemory.Data;
using Tauron.Application.Files.VirtualFiles.LocalFileSystem;

namespace Tauron.Application.Deployment.Server.Engine.Impl
{
    public sealed class FileSystem : IFileSystem, IDisposable
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDisposable _subscription;
        private LocalSettings _settings;
        private IDirectory _repositoryRoot;

        public FileSystem(IOptionsMonitor<LocalSettings> localOptions, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _settings = localOptions.CurrentValue;
            _subscription = localOptions.OnChange(ls =>
                                                  {
                                                      _settings = ls;
                                                      UpdatePaths();
                                                  });
            UpdatePaths();
        }

        public IDirectory RepositoryRoot
        {
            get => _repositoryRoot;
            private set => _repositoryRoot = value;
        }

        private void UpdatePaths()
        {
            var basePath = _settings.ServerFileMode switch
            {
                ServerFileMode.Unkowen => "Invalid",
                ServerFileMode.ApplicationData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tauron", "Repositorys"),
                ServerFileMode.ContentRoot => Path.Combine(_webHostEnvironment.ContentRootPath, "AppData", "Repositorys"),
                _ => throw new ArgumentOutOfRangeException()
            };

            switch (basePath)
            {
                case "Invalid":
                    Interlocked.Exchange(ref _repositoryRoot, new InMemoryFileSystem(basePath, basePath, new DataDirectory(basePath)));
                    break;
                default:
                    Interlocked.Exchange(ref _repositoryRoot, new LocalFileSystem(basePath));
                    break;
            }
        }

        public void Dispose() 
            => _subscription.Dispose();
    }
}