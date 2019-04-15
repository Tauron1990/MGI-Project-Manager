using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface IOperationRepository
    {
        Task<IEnumerable<OperationEntity>> GetAllOperations();

        Task<OperationEntity> Find(string id);

        Task AddOperation(OperationEntity entity);

        Task CompledOperation(string id);

        Task Remove(string id);

        Task UpdateOperation(string id, Action<OperationEntity> update);
    }
}