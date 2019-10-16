using System.Threading.Tasks;
using CQRSlite.Commands;
using Tauron.CQRS.Services.Specifications;

namespace Tauron.CQRS.Services
{
    public abstract class SpecificationCommandHandler<TType> : ISpecificationCommandHandler<TType> 
        where TType : ICommand
    {
        public abstract Task Handle(TType command, string error);

        protected abstract ISpecification GetSpecification(GenericSpecification<TType> helper);

        public ISpecification GetSpecification() 
            => GetSpecification(new GenericSpecification<TType>());
    }
}