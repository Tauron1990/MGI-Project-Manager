using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public class SimpleBuildPrameter : BuildParameter
    {
        private readonly IExport _export;

        public SimpleBuildPrameter([CanBeNull] IExport export)
        {
            _export = export;
        }

        public SimpleBuildPrameter([NotNull] object obj)
        {
            _export = DefaultExportFactory.Factory.CreateAnonymosWithTarget(obj.GetType(), obj);
        }

        protected internal override IExport CreateExport()
        {
            return _export;
        }
    }
}