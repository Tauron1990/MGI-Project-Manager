using System.Threading.Tasks;
using CQRSlite.Commands;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Specifications;

namespace Tauron.CQRS.Services
{
    public interface ISpecificationCommandHandler<in TCommand> : ISpecificationProviderBase
        where TCommand : ICommand
    {
        Task Handle(TCommand command, string error);
    }
}