namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public sealed class NewSelectorItem : SelectorItemBase
    {
        public NewSelectorItem(string name) => Name = name;

        public override string Name { get; }
        public override ItemType ItemType { get; } = ItemType.New;
    }
}