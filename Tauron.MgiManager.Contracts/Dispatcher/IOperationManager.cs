using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public interface IOperationManager
    {
        Task<string> AddOperation(OperationSetup op);

        Task UpdateOperation(string id, Action<IDictionary<string, string>> toUpdate);

        Task<IReadOnlyDictionary<string, string>> SearchOperation(string id);
        
        Task<IEnumerable<string>> ExecuteNext(string id);

        Task RemoveAction(string id);

        Task CleanUpExpiryOperation();

        Task<string[]> GetOperations(Predicate<OperationFilter> filter);

        Task<IDictionary<string, string>> GetContext(string id);
    }
}