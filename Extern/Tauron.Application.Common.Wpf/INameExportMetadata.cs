using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface INameExportMetadata
    {
        [NotNull]
        string Name { get; }
    }

    internal abstract class NameExportMetadataContracts : INameExportMetadata
    {
        public string Name => null;
    }
}