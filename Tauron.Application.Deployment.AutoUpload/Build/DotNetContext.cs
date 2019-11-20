using System.IO;
using Functional;

namespace Tauron.Application.Deployment.AutoUpload.Build
{
    public sealed class DotNetContext
    {
        private const string DotNetLocation = @"C:\Program Files\dotnet\dotnet.exe";

        public static Option<DotNetContext> Create() 
            => Option
                .Where(File.Exists(DotNetLocation))
                .Bind(u => Option.Some(new DotNetContext()));
    }
}