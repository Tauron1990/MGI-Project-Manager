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
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ExpressionEvaluatorHandler> _logger;

        public ExpressionEvaluatorHandler(ISession session, ILogger<ExpressionEvaluatorHandler> logger)
        {
            _session = session;
            _logger = logger;
        }

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
                _logger.LogInformation($"Evaluate Expression: {command.Input}");
                var result = ExpressionEvaluator.Evaluate<object>(command.Input).ToString();
                _logger.LogInformation($"Evaluate Result: {command.Input} = {result}");

                ExpressionAggregate aggregate;
                try
                {
                    aggregate = await _session.Get<ExpressionAggregate>(ExpressionsNamespaces.ExpressionAggregate);
                }
                catch (AggregateNotFoundException)
                {
                    aggregate = new ExpressionAggregate();
                    await _session.Add(aggregate);
                }

                aggregate.AddExpression(new ExpressionElevatedEvent(command.Input, result, false));

                await _session.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on Evaluate: {command.Input}");
                await _session.PublishEvent(new ExpressionElevatedEvent(command.Input, e.Message, true));
            }
        }
    }
}