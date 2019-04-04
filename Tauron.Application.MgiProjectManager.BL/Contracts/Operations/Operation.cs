using System;
using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public class Operation
    {
        public string OperationId { get; }

        public string OperationType { get; }
        
        public string CurrentOperation { get; }

        public DateTime ExpiryDate { get; }

        public IDictionary<string, string> OperationContext { get; }

        public Operation(string operationId, string currentOperation, string operationType, IDictionary<string, string> operationContext, DateTime expiryDate)
        {
            OperationId = operationId;
            CurrentOperation = currentOperation;
            OperationType = operationType;
            OperationContext = operationContext;
            ExpiryDate = expiryDate;
        }
    }
}