using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer
{
    [PublicAPI]
    public sealed class ExportRepositoryExtenderAttribute : ExportAttribute
    {
        public ExportRepositoryExtenderAttribute() : base(typeof(IRepositoryExtender))
        {
        }
    }
}