using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tauron.MgiProjectManager.Dispatcher.Model
{
    public class OperationFilter
    {
        public string OperationType { get; }

        public string CurrentOperation { get; }

        public string OperationId { get; }

        public IReadOnlyDictionary<string, string> Context { get; }

        private OperationFilter(string operationType, string currentOperation, string operationId, IReadOnlyDictionary<string, string> context)
        {
            OperationType = operationType;
            CurrentOperation = currentOperation;
            OperationId = operationId;
            Context = context;
        }

        public static OperationFilter FromOperation(Operation op) 
            => new OperationFilter(op.OperationType, op.CurrentOperation, op.OperationId, new ReadOnlyDictionary<string, string>(op.OperationContext));
    }
}