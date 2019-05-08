using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Contexts;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    [Export(typeof(ILoggingDb))]
    public class LoggingDb : ILoggingDb
    {
        private readonly LoggingDbContext _context;

        public LoggingDb(LoggingDbContext context) 
            => _context = context;

        public async Task LimitCount(int count)
        {
            var amount = await _context.Events.CountAsync();
            if (count < amount)
            {
                _context.Events.RemoveRange(_context.Events.OrderBy(m => m.Timestamp).Take(amount - count).AsEnumerable());
                await _context.SaveChangesAsync();
            }
        }
    }
}