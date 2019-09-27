using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Installation.Core;
using ServiceManager.Installation.Tasks.Ui;

namespace ServiceManager.Installation.Tasks
{
    public sealed class NameSelectionTask : InstallerTask
    {
        private NameSelectionModel _nameSelectionModel;
        private object _content;

        public override object Content => _content;

        public override string Title => "Service Name";

        public override Task Prepare(InstallerContext context)
        {
            _nameSelectionModel = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelectionModel>();
            _content = context.ServiceScope.ServiceProvider.GetRequiredService<NameSelection>();

            return Task.CompletedTask;
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            var zip = context.PackageArchive;
            if (zip == null) return "Datei ist keine Zip Datei";

            string name = GetName(zip);

            if (!string.IsNullOrWhiteSpace(name))
            {
                context.ServiceName = name;
                return null;
            }

            await _nameSelectionModel.Wait();

            name = _nameSelectionModel.NameText;

            if (string.IsNullOrWhiteSpace(name))
                return "Kein korrekter name wurde angegeben";

            context.ServiceName = name;

            return null;
        }

        private string GetName(ZipArchive zipArchive)
        {

        }
    }
}