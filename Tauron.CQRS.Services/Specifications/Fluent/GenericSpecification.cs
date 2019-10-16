using System;

namespace Tauron.CQRS.Services.Specifications
{
    public sealed class GenericSpecification<TType>
    {
        public ISpecification Simple(Func<TType, bool> eval, string msg)
            => SpecOps.Simple(eval, msg);
    }
}