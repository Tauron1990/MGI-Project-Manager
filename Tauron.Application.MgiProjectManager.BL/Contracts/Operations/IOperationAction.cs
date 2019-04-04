using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IOperationAction
    {
        string Name { get; }

        Task<Operation[]> Execute(Operation op);

        Task<bool> Remove(Operation op);

        Task Error(Operation op, Exception e);
    }
}