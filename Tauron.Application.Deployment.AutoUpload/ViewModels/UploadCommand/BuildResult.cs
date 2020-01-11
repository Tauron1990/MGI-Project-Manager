using System;
using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    public class BuildResult
    {
        private readonly BuildOperationContext _context;

        public BuildResult(BuildOperationContext context) => _context = context;

        public async Task Do(Func<string, Version, Task> suceed, Func<BuildFailed, Task> failed)
        {
            if (_context.Failed == null)
                await suceed(_context.Location, _context.AssemblyVersion);
            else
                await failed(_context.Failed);
        }
    }
}