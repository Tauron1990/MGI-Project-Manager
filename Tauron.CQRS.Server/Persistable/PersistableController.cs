using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tauron.CQRS.Common.Persistable;
using Tauron.CQRS.Server.Core;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Persistable
{
    [ApiController]
    [Route("[controller]")]
    public class PersistableController : Controller
    {
        private readonly IApiKeyStore _apiKeyStore;
        private readonly DispatcherDatabaseContext _context;

        public PersistableController(IApiKeyStore apiKeyStore, DispatcherDatabaseContext context)
        {
            _apiKeyStore = apiKeyStore;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ObjectStade>> Get(string id, string apikey)
        {
            if (!await _apiKeyStore.Validate(apikey))
                return base.Forbid();

            var entity = await _context.ObjectStades.AsNoTracking().FirstOrDefaultAsync(o => o.Identifer == id);

            return entity == null ? new ObjectStade{ Data = string.Empty, Identifer = id} : new ObjectStade {Data = entity.Data, Identifer = entity.Identifer};
        }

        [HttpPut]
        public async Task<IActionResult> Put(ObjectStade stade, string apiKey)
        {
            if (!await _apiKeyStore.Validate(apiKey))
                return base.Forbid();

            string id = stade.Identifer;

            var entity = await _context.ObjectStades.FirstOrDefaultAsync(o => o.Identifer == id);

            if (entity != null)
                entity.Data = stade.Data;
            else
                _context.ObjectStades.Add(new ObjectStadeEntity {Data = stade.Data, Identifer = stade.Identifer});

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}