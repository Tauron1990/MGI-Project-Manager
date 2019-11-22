using System.IO;
using Octokit;
using Tauron.Application.Deployment.AutoUpload.Core;

namespace Tauron.Application.Deployment.AutoUpload.Build
{
    public sealed class ApplicationContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public Settings Settings { get; } = Settings.Create();

        public GitHubClient GitHubClient { get; }

        private ApplicationContext() 
            => GitHubClient = new GitHubClient(new ProductHeaderValue("Tauron.Application.Deployment.AutoUpload"));

        public static ApplicationContext? Create()
        {
            return File.Exists(DotNetLocation) ? new ApplicationContext() : null;
        }
    }
}