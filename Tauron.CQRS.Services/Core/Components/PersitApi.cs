using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto.Persistable;
using Tauron.CQRS.Services.Data.Database;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly]
    public class PersitApi : IPersistApi
    {
        private readonly EventSourceContext _databaseContext;

        public PersitApi(EventSourceContext databaseContext) => _databaseContext = databaseContext;

        public async Task<ObjectStade> Get(string id)
        {
            var ent = await _databaseContext
                         .ObjectStades
                         .AsNoTracking()
                         .FirstOrDefaultAsync(s => s.Identifer == id);

            if (ent == null) return null;

            return new ObjectStade
                   {
                       Data = JToken.Parse(ent.Data),
                       Identifer = ent.Identifer,
                       OriginalType = ent.OriginType
                   };
        }

        public async Task Put(ObjectStade stade)
        {
            var ent = await _databaseContext
                         .ObjectStades
                         .FirstOrDefaultAsync(s => s.Identifer == stade.Identifer);

            if (ent == null)
            {
                _databaseContext.ObjectStades.Add(new ObjectStadeEntity
                                                  {
                                                      Data = stade.Data.ToString(),
                                                      Identifer = stade.Identifer,
                                                      OriginType = stade.OriginalType
                                                  });
            }
            else
            {
                ent.Data = stade.Data.ToString();
                ent.OriginType = stade.OriginalType;
            }

            await _databaseContext.SaveChangesAsync();
        }
    }
}