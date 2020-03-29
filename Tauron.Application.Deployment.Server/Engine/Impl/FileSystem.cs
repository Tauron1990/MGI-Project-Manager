using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Tauron.Application.Deployment.Server.Engine.Impl
{
    public sealed class FileSystem : IFileSystem, IDisposable
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDisposable _subscription;
        private LocalSettings _settings;
        private string _repositoryRoot;

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

        public string RepositoryRoot
        {
            get => _repositoryRoot;
            private set => _repositoryRoot = value;
        }

        private void UpdatePaths()
        {
            var basePath = _settings.ServerFileMode switch
            {
                ServerFileMode.Unkowen => "Invalid",
                ServerFileMode.ApplicationData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tauron"),
                ServerFileMode.ContentRoot => Path.Combine(_webHostEnvironment.ContentRootPath, "AppData"),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (basePath == "Invalid")
                return;


            Interlocked.Exchange(ref _repositoryRoot, Path.Combine(basePath, "Repositorys"));
        }

        public void Dispose() 
            => _subscription.Dispose();
    }
}