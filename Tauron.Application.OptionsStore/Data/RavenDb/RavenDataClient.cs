﻿using System;
using System.Threading;
using Raven.Client.Documents;

namespace Tauron.Application.OptionsStore.Data.RavenDb
{
    public sealed class RavenDataClient : IDataClient
    {
        private readonly Lazy<IDocumentStore> _documetStore;

        public RavenDataClient(Func<IServiceProvider, IDocumentStore> documetStore, IServiceProvider serviceProvider) 
            => _documetStore = new Lazy<IDocumentStore>(() => documetStore(serviceProvider), LazyThreadSafetyMode.ExecutionAndPublication);

        public IOptionDataCollection GetCollection(string name)
        {
            if (name.Contains("-"))
                throw new ArgumentException(" \"- \" in Names are not allowed");

            return new RavenCollection(name, _documetStore);
        }
    }
}