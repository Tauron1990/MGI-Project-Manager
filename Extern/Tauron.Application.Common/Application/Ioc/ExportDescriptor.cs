using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public class ExportDescriptor
    {
        public ExportDescriptor([NotNull] ExportMetadata meta)
        {
            Meta = meta;
        }

        [NotNull] public ExportMetadata Meta { get; set; }
    }
}