using System.Threading.Tasks;
using CQRSlite.Commands;
using Tauron.CQRS.Services;

namespace Tauron.TestHelper
{
    public static class SCHTest
    {
        public static SpecificationCommandHandlerTest<TCommand> Create<TCommand>(ISpecificationCommandHandler<TCommand> handler)
            where TCommand : ICommand =>
            new SpecificationCommandHandlerTest<TCommand>(handler);
    }

    public sealed class SpecificationCommandHandlerTest<TCommand>
        where TCommand : ICommand
    {
        private readonly ISpecificationCommandHandler<TCommand> _handler;

        public SpecificationCommandHandlerTest(ISpecificationCommandHandler<TCommand> handler) => _handler = handler;

        public Task Handle(TCommand command)
        {
            var spec = _handler.GetSpecification();
            string error = null;
            if (spec != null && !spec.IsSatisfiedBy(command))
                error = spec.Message;


            return _handler.Handle(command, error);
        }
    }
}