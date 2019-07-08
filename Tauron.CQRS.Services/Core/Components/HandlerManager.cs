using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Services.Core.Components
{
    public class HandlerManager : IHandlerManager
    {
        private readonly IDispatcherClient _client;

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client) 
            => _client = client;

        public async Task Init(CancellationToken token)
        {


            await _client.Start(token);
        }
    }
}