using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Specifications
{
    public abstract class SpecificationBase<TTarget> : ISpecification
    {
        public abstract string Message { get; }

        public bool IsSatisfiedBy(object obj)
        {
            if (obj is TTarget target)
                return IsSatisfiedBy(target);

            return false;
        }

        protected abstract bool IsSatisfiedBy(TTarget target);
    }
}