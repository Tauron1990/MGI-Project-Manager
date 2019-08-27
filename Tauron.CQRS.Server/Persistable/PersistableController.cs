using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;
using Tauron.CQRS.Server.Core;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Persistable
{
    [ApiController]
    [Route("Api/[controller]")]
    public class PersistableController : ControllerBase
    {
        private readonly IApiKeyStore _apiKeyStore;
        private readonly DispatcherDatabaseContext _context;

        public PersistableController(IApiKeyStore apiKeyStore, DispatcherDatabaseContext context)
        {
            _apiKeyStore = apiKeyStore;
            _context = context;
        }

        [HttpGet()]
        public async Task<ActionResult<ObjectStade>> Get([FromBody] ApiObjectId id)
        {
            if (!await _apiKeyStore.Validate(id.ApiKey))
                return base.Forbid();

            string realId = id.Id;

            var entity = await _context.ObjectStades.AsNoTracking().FirstOrDefaultAsync(o => o.Identifer == realId);

            return entity == null
                ? new ObjectStade {Identifer = realId}
                : new ObjectStade
                {
                    Data = JToken.Parse(entity.Data), // TypeFactory.Create(entity.OriginType, entity.Data) as IObjectData,
                    Identifer = entity.Identifer,
                    OriginalType = entity.OriginType
                };
        }

        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] ApiObjectStade stade)
        {
            if (!await _apiKeyStore.Validate(stade.ApiKey))
                return base.Forbid();

            string id = stade.ObjectStade.Identifer;

            var entity = await _context.ObjectStades.FirstOrDefaultAsync(o => o.Identifer == id);

            if (entity != null)
                entity.Data = stade.ObjectStade.Data.ToString();
            else
            {
                _context.ObjectStades.Add(new ObjectStadeEntity
                                          {
                                              Data = stade.ObjectStade.Data.ToString(),
                                              Identifer = stade.ObjectStade.Identifer,
                                              OriginType = stade.ObjectStade.OriginalType
                                          });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}