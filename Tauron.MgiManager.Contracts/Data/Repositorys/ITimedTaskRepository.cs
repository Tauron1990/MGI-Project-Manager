using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface ITimedTaskRepository
    {
        Task<IEnumerable<TimedTaskEntity>> GetTasks();
        Task<TimedTaskEntity> UpdateTime(string name);
    }
}