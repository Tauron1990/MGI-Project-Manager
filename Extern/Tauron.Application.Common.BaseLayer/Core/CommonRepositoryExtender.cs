using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    [PublicAPI]
    public abstract class CommonRepositoryExtender<TDbContext> : IRepositoryExtender
        where TDbContext : DbContext, new()
    {
        protected CommonRepositoryExtender()
            : this(new CommonFactory<TDbContext>())
        {
            
        }

        protected CommonRepositoryExtender(IDatabaseFactory factory)
        {
            DatabaseFactory = factory;
        }

        public IDatabaseFactory DatabaseFactory { get; }

        public abstract IEnumerable<(Type, Type)> GetRepositoryTypes();
    }
}