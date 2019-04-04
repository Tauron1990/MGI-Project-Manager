using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Server.Data.Repository
{
    public interface ITimedTaskRepository
    {
        Task<IEnumerable<TimedTaskEntity>> GetTaskAsync();
        Task<TimedTaskEntity> UpdateTime(string name);
    }
}