using System;
using System.Globalization;
using System.Threading.Tasks;
using Calculator.Shared;
using Calculator.Shared.Commands;
using Calculator.Shared.Events;
using CalculatorService.Aggregates;
using CodingSeb.ExpressionEvaluator;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;
using Tauron.CQRS.Services.Specifications;

namespace CalculatorService.CommandHandlers
{
    [CQRSHandler]
    public class ExpressionEvaluatorHandler : ISpecificationCommandHandler<RunExpressionCommand>
    {
        private static readonly ExpressionEvaluator ExpressionEvaluator = new ExpressionEvaluator();

        private readonly ISession _session;

        public ExpressionEvaluatorHandler(ISession session) 
            => _session = session;

        public ISpecification GetSpecification()
        {
            return SpecificationFactory<RunExpressionCommand>
               .GetSpecification(() =>
                                     SpecOps.Simple<RunExpressionCommand>(
                                         command => Task.FromResult(!string.IsNullOrWhiteSpace(command.Input)),
                                         "Keine Expression wurde Angegeben."), 
                                 nameof(ExpressionEvaluatorHandler));
        }

        public async Task Handle(RunExpressionCommand command, string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                await _session.PublishEvent(new ExpressionElevatedEvent(command.Input, error, true));
                return;
            }

            try
            {
                var result = ExpressionEvaluator.Evaluate<double>(command.Input).ToString(CultureInfo.InvariantCulture);

                ExpressionAggregate aggregate;
                try
                {
                    aggregate = await _session.Get<ExpressionAggregate>(ExpressionsNamespaces.ExpressionAggregate);
                }
                catch (AggregateNotFoundException)
                {
                    aggregate = new ExpressionAggregate();
                }

                aggregate.AddExpression(new ExpressionElevatedEvent(command.Input, result, false));

                await _session.Commit();
            }
            catch (Exception e)
            {
                await _session.PublishEvent(new ExpressionElevatedEvent(command.Input, e.Message, true));
            }
        }
    }
}