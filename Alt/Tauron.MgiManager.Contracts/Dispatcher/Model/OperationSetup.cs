using System;
using System.Collections.Generic;

namespace Tauron.MgiProjectManager.Dispatcher.Model
{
    public class OperationSetup
    {
        public string OperationType { get; }

        public string CurrentOperation { get; }

        public DateTime ExpiryDate { get; }

        public IDictionary<string, string> OperationContext { get; }

        public OperationSetup( string currentOperation, string operationType, IDictionary<string, string> operationContext, DateTime expiryDate)
        {
            CurrentOperation = currentOperation;
            OperationType = operationType;
            OperationContext = operationContext;
            ExpiryDate = expiryDate;
        }

        public Operation ToOperation() 
            => new Operation(Guid.NewGuid().ToString("D"), CurrentOperation, OperationType, OperationContext, ExpiryDate);
    }
}