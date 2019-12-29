using System.Threading.Tasks;
using Catel.Collections;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.SoftwareRepo;
using Tauron.Application.SoftwareRepo.Data;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionShowRepoViewModel))]
    public sealed class VersionShowRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public FastObservableCollection<ApplicationEntry> ApplicationEntries { get; } = new FastObservableCollection<ApplicationEntry>();

        protected override async Task InitializeAsync()
        {
            if (Context.VersionRepository == null || !SoftwareRepository.IsValid(Context.VersionRepository.RealPath))
            {
                Name = "Invalid";
                return;
            }

            var repo = await SoftwareRepository.Read(Context.VersionRepository.RealPath);

            Name = repo.ApplicationList.Name;
            Description = repo.ApplicationList.Description;

            ApplicationEntries.AddItems(repo.ApplicationList.ApplicationEntries);

            await base.InitializeAsync();
        }
    }
}