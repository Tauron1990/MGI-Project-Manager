using Microsoft.Extensions.DependencyInjection;

namespace TestHelpers.Core
{
    public abstract class ServiceEntry
    {
        public abstract void Register(IServiceCollection collection);

        public abstract void Assert();
    }
}