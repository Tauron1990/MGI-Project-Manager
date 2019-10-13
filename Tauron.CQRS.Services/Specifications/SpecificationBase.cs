using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Specifications
{
    public abstract class SpecificationBase<TTarget> : ISpecification
    {
        public abstract string Message { get; }

        public async Task<bool> IsSatisfiedBy(object obj)
        {
            if (obj is TTarget target)
                return await IsSatisfiedBy(target);

            return false;
        }

        protected abstract Task<bool> IsSatisfiedBy(TTarget target);
    }
}