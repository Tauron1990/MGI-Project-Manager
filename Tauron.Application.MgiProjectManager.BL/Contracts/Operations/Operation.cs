using System;
using System.Collections.Generic;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public class Operation
    {
        public string OperationId { get; }

        public string NextOperation { get; set; }

        public DateTime ExpiryDate { get; set; }

        public IDictionary<string, string> OperationContext { get; }

        public Operation(string operationId, string nextOperation, IDictionary<string, string> operationContext)
        {
            OperationId = operationId;
            NextOperation = nextOperation;
            OperationContext = operationContext;
        }
    }
}