using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.CQRS.Services.Specifications
{
    public static class SpecificationFactory<TTarget>
    {
        private static readonly Dictionary<string, ISpecification> _specifications = new Dictionary<string, ISpecification>();

        public static ISpecification GetSpecification(Func<ISpecification> factory, string name)
        {
            if (_specifications.TryGetValue(name, out var specification)) return specification;

            specification = factory();

            _specifications[name] = specification;

            return specification;
        }
    }
}