using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    public sealed class VersionRepoItem : SelectorItemBase
    {
        public VersionRepoItem(VersionRepository versionRepository) => VersionRepository = versionRepository;

        public VersionRepository VersionRepository { get; }
        public override string Name => VersionRepository.Name;
        public override ItemType ItemType { get; } = ItemType.Item;
    }
}