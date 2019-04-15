using System;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Dispatcher.Actions
{
    public interface IOperationAction
    {
        string Name { get; }

        Task<OperationSetup[]> Execute(Operation op);

        Task PostExecute(Operation op);

        Task<bool> Remove(Operation op);

        Task Error(Operation op, Exception e);
    }
}