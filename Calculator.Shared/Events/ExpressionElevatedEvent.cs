using System;
using Tauron.CQRS.Services;

namespace Calculator.Shared.Events
{
    public sealed class ExpressionElevatedEvent : BaseEvent
    {
        public override Guid Id { get; set; } = ExpressionsNamespaces.ExpressionAggregate;

        public string Expression { get; set; }

        public string Result { get; set; }

        public bool Error { get; set; }

        public ExpressionElevatedEvent()
        {
            
        }

        public ExpressionElevatedEvent(string expression, string result, bool error)
        {
            Expression = expression;
            Result = result;
            Error = error;
        }
    }
}