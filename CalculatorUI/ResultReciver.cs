using System;
using System.Threading.Tasks;
using Calculator.Shared.Events;
using CQRSlite.Events;
using Tauron.CQRS.Services.Extensions;

namespace CalculatorUI
{
    [CQRSHandler]
    public class ResultReciver : IEventHandler<ExpressionElevatedEvent>
    {
        public static event Action<ExpressionElevatedEvent> Result;

        public Task Handle(ExpressionElevatedEvent message)
        {
            Result?.Invoke(message);

            return Task.CompletedTask;
        }
    }
}