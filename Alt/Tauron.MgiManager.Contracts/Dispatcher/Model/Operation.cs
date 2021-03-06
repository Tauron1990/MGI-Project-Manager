﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Dispatcher.Model
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

        public OperationEntity ToOperationEntity()
        {
            return new OperationEntity
            {
                OperationId = OperationId,
                OperationType = OperationType,
                ExpiryDate = ExpiryDate,
                Context = new List<OperationContextEntity>(OperationContext.Select(pair => new OperationContextEntity {Name = pair.Key, Value = pair.Value}))
            };
        }
    }
}