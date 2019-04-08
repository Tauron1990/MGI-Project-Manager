using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Server.Data.Repository
{
    public interface IOperationRepository
    {
        Task<IEnumerable<OperationEntity>> GetAllOperations();

        Task AddOperation(OperationEntity entity);

        Task CompledOperation(string id);

        Task Remove(string id);

        Task UpdateOperation(string id, Action<OperationEntity> update);
    }
}