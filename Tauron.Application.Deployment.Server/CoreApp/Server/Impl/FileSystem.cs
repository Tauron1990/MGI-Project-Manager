using System;
using System.ComponentModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Tauron.Application.Deployment.Server.CoreApp.Server.Impl
{
    public sealed class FileSystem : IFileSystem
    {
        private readonly DatabaseOptions _databaseOptions;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public string RepositoryRoot { get; private set; }

        public FileSystem(DatabaseOptions databaseOptions, IWebHostEnvironment webHostEnvironment)
        {
            _databaseOptions = databaseOptions;
            _webHostEnvironment = webHostEnvironment;
            databaseOptions.PropertyChanged += DatabaseOptionsOnPropertyChanged;
            UpdatePaths();
        }

        private void DatabaseOptionsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_databaseOptions.ServerFileMode))
                UpdatePaths();
        }

        private void UpdatePaths()
        {
            string basePath = string.Empty;

            switch (_databaseOptions.ServerFileMode)
            {
                case ServerFileMode.Unkowen:
                    basePath = "Invalid";
                    break;
                case ServerFileMode.ApplicationData:
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tauron");
                    break;
                case ServerFileMode.ContentRoot:
                    basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "AppData");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(basePath ==  "Invalid") 
                return;

            RepositoryRoot = Path.Combine(basePath, "Repositorys");
        }
    }
}