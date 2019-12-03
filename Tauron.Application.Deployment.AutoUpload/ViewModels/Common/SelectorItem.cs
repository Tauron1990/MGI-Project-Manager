namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Common
{
    public sealed class SelectorItem<TType> : SelectorItemBase
        where TType : INameable
    {
        public TType Target { get; }
        public override string Name => Target.Name;
        public override ItemType ItemType { get; } = ItemType.Item;

        public SelectorItem(TType target) => Target = target;
    }
}