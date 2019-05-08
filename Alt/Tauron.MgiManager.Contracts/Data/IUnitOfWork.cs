// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

using System;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Repositorys;

namespace Tauron.MgiProjectManager.Data
{
    public interface IUnitOfWork
    {
        IFileRepository FileRepository { get; }
        ITimedTaskRepository TimedTaskRepository { get; }

        IOperationRepository OperationRepository { get; }

        Task<int> SaveChanges();
    }
}
