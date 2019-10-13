using System.Collections.Generic;
using Calculator.Shared.Dto;
using Calculator.Shared.Events;
using JetBrains.Annotations;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core.Components;

namespace CalculatorService.Aggregates
{
    public class ExpressionAggregate : CoreAggregateRoot
    {
        public Queue<ExpressionEntry> Expressions
        {
            get => GetValue<Queue<ExpressionEntry>>();
            set => SetValue(value);
        }

        public ExpressionAggregate() => Expressions = new Queue<ExpressionEntry>();

        [UsedImplicitly]
        private void Apply(ExpressionElevatedEvent expressionElevated)
        {
            var queue = Expressions;

            if (queue.Count > 100)
                queue.Dequeue();

            queue.Enqueue(new ExpressionEntry(expressionElevated.Expression, expressionElevated.Result));
        }

        public void AddExpression(ExpressionElevatedEvent @event)
            => ApplyChange(@event);
    }
}