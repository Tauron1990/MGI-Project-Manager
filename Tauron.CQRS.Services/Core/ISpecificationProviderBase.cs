using Tauron.CQRS.Services.Specifications;

namespace Tauron.CQRS.Services.Core
{
    public interface ISpecificationProviderBase
    {
        ISpecification GetSpecification();
    }
}