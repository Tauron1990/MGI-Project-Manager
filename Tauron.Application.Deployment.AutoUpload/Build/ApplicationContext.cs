using System.IO;
using Tauron.Application.Deployment.AutoUpload.Core;

namespace Tauron.Application.Deployment.AutoUpload.Build
{
    public sealed class ApplicationContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public Settings Settings { get; } = Settings.Create();

        public static ApplicationContext? Create()
        {
            return File.Exists(DotNetLocation) ? new ApplicationContext() : null;
        }
    }
}