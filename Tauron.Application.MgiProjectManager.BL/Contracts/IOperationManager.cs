using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IOperationManager
    {
        IEnumerable<Operation> Operations { get; }

        Task AddOperation(Operation op);

        Task UpdateOperation(Operation op);

        Task<Operation> SearchOperation(string id);

        Task<IEnumerable<Operation>> ExecuteNext(Operation op);

        Task RemoveAction(Operation op);

        Task CleanUpExpiryOperation();
    }
}