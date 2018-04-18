using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer
{
    [PublicAPI]
    public class InjectRepositoryFactoryAttribute : InjectAttribute
    {
        public InjectRepositoryFactoryAttribute()
            : base(typeof(RepositoryFactory))
        {
        }
    }
}