namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public abstract class SelectorItemBase
    {
        public abstract string Name { get; }

        public abstract ItemType ItemType { get; }
    }
}