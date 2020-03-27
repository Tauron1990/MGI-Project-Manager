using System;
using System.ComponentModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Tauron.Application.Deployment.Server.Engine.Impl
{
    public sealed class FileSystem : IFileSystem
    {
        private readonly DatabaseOptions _databaseOptions;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileSystem(DatabaseOptions databaseOptions, IWebHostEnvironment webHostEnvironment)
        {
            _databaseOptions = databaseOptions;
            _webHostEnvironment = webHostEnvironment;
            databaseOptions.PropertyChanged += DatabaseOptionsOnPropertyChanged;
            UpdatePaths();
        }

        public string RepositoryRoot { get; private set; }

        private void DatabaseOptionsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_databaseOptions.ServerFileMode))
                UpdatePaths();
        }

        private void UpdatePaths()
        {
            var basePath = _databaseOptions.ServerFileMode switch
            {
                ServerFileMode.Unkowen => "Invalid",
                ServerFileMode.ApplicationData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tauron"),
                ServerFileMode.ContentRoot => Path.Combine(_webHostEnvironment.ContentRootPath, "AppData"),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (basePath == "Invalid")
                return;

            RepositoryRoot = Path.Combine(basePath, "Repositorys");
        }
    }
}