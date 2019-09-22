using System.Threading.Tasks;
using CQRSlite.Commands;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand> 
        where TCommand : ICommand
    {
        public IdGenerator IdGenerator => IdGenerator.Generator;

        public abstract Task Handle(TCommand message);
    }
}