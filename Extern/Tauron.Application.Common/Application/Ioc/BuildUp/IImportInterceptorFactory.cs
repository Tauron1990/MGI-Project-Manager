using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public interface IImportInterceptorFactory
    {
        [CanBeNull]
        IImportInterceptor CreateInterceptor([NotNull] ExportMetadata export);
    }
}