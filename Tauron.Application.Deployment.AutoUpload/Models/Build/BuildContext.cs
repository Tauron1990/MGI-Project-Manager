using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildContext), ServiceLifetime.Singleton)]
    public sealed class BuildContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public bool CanBuild => File.Exists(DotNetLocation);

        public async Task TryBuild(RegistratedRepository? repository)
        {
            var arguments = new StringBuilder()
               .Append()
               .Append("publish ")
               .Append(Context.RegistratedRepository?.ProjectName).Append(" ")
               .Append($"-o {targetPath} ")
               .Append("-c Release")
               .Append("-v d");
        }
    }
}