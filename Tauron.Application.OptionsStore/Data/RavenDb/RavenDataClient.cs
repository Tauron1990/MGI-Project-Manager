using System;
using System.Threading;
using Tauron.Application.Data.Raven;

namespace Tauron.Application.OptionsStore.Data.RavenDb
{
    public sealed class RavenDataClient : IDataClient
    {
        private readonly Lazy<IDatabaseRoot> _documetStore;

        public RavenDataClient(Func<IServiceProvider, IDatabaseRoot> documetStore, IServiceProvider serviceProvider)
        {
            _documetStore = new Lazy<IDatabaseRoot>(() => documetStore(serviceProvider), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public IOptionDataCollection GetCollection(string name)
        {
            if (name.Contains("-"))
                throw new ArgumentException(" \"- \" in Names are not allowed");

            return new RavenCollection(name, _documetStore);
        }
    }
}