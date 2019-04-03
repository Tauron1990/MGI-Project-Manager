using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IOperationManager
    {
        IEnumerable<Operation> Operations { get; }

        void AddOperation(Operation op);
    }
}