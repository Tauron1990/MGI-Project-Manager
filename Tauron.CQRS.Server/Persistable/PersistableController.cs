using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;
using Tauron.CQRS.Common.Dto.TypeHandling;
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
                    Data = TypeFactory.Create(entity.OriginType, entity.Data) as IObjectData,
                    Identifer = entity.Identifer
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
                entity.Data = TypeFactory.Serialize(stade.ObjectStade.Data);
            else
                _context.ObjectStades.Add(new ObjectStadeEntity
                {
                    Data = TypeFactory.Serialize(stade.ObjectStade.Data),
                    Identifer = stade.ObjectStade.Identifer,
                    OriginType = stade.ObjectStade.Data.GetType().AssemblyQualifiedName
                });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}