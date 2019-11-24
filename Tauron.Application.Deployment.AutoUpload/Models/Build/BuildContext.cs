using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildContext), ServiceLifetime.Singleton)]
    public sealed class BuildContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public bool CanBuild => File.Exists(DotNetLocation);
    }
}