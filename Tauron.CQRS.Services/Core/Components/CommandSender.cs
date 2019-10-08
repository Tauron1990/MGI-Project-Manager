using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;

namespace Tauron.CQRS.Services.Core.Components
{
    public class CommandSender : ICommandSender
    {
        private readonly IDispatcherClient _dispatcher;

        public CommandSender(IDispatcherClient api) => _dispatcher = api;

        public async Task Send<T>(T command, CancellationToken cancellationToken = new CancellationToken()) 
            where T : class, ICommand 
            => await _dispatcher.Send(command, cancellationToken).ConfigureAwait(false);
    }
}