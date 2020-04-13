using System.Threading.Tasks;
using Catel.Collections;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;
using Tauron.Application.SoftwareRepo.Data;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    [ServiceDescriptor(typeof(VersionShowRepoViewModel))]
    public sealed class VersionShowRepoViewModel : OperationViewModel<VersionRepoContext>
    {
        private readonly IRepoFactory _repoFactory;
        private readonly ISLogger<VersionShowRepoViewModel> _logger;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public FastObservableCollection<ApplicationEntry> ApplicationEntries { get; } = new FastObservableCollection<ApplicationEntry>();

        public VersionShowRepoViewModel(IRepoFactory repoFactory, ISLogger<VersionShowRepoViewModel> logger)
        {
            _repoFactory = repoFactory;
            _logger = logger;
        }

        protected override async Task InitializeAsync()
        {
            _logger.Information("Initialize Show Repository View Data");
            
            if (Context.VersionRepository == null || !_repoFactory.IsValid(Context.VersionRepository.RealPath))
            {
                _logger.Warning("Invalid Repository Data: {Context}, {Path}", Context?.ToString(), Context?.VersionRepository?.RealPath);
                Name = "Invalid";
                return;
            }

            var repo = await _repoFactory.Read(Context.VersionRepository.RealPath);

            Name = repo.ApplicationList.Name;
            Description = repo.ApplicationList.Description;

            ApplicationEntries.AddItems(repo.ApplicationList.ApplicationEntries);

            await base.InitializeAsync();
        }

        [CommandTarget]
        public async Task Return()
        {
            await OnReturn();
        }
    }
}