using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public class CommonFactory<TDbContext> : IDatabaseFactory
        where TDbContext : DbContext, new()
    {
        public virtual string Id { get; } = "Common";

        public virtual IDatabase CreateDatabase()
        {
            return new CommonDatabase<TDbContext>(new TDbContext());
        }
    }
}