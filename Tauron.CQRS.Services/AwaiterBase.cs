using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.CQRS.Services
{
    
    public abstract class AwaiterBase<TMessage, TRespond>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AwaiterBase(IServiceScopeFactory scopeFactory) 
            => _scopeFactory = scopeFactory;

        protected abstract TMessage Create();

        protected abstract Task Handle(TRespond respond);

        public void Send(TMessage msg)
        {

        }
    }
}