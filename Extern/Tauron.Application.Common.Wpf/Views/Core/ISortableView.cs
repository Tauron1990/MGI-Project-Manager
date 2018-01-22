namespace Tauron.Application.Views.Core
{
    public interface ISortableViewExportMetadata : INameExportMetadata
    {
        int Order { get; }
    }
}