using System.Collections.Generic;
using System.Threading.Tasks;
using Calculator.Shared;
using Calculator.Shared.Dto;
using Calculator.Shared.Querys;
using CalculatorService.Aggregates;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;

namespace CalculatorService.ReadModels
{
    [CQRSHandler]
    public sealed class ExpressionsReadModel : ReadModelBase<ExpressionResult, ExpressionsQuery>
    {
        private readonly ISession _session;
        private readonly ILogger<ExpressionsReadModel> _logger;

        public ExpressionsReadModel(IDispatcherClient client, ISession session, ILogger<ExpressionsReadModel> logger) 
            : base(client)
        {
            _session = session;
            _logger = logger;
        }

        protected override async Task<ExpressionResult> Query(ExpressionsQuery query)
        {
            _logger.LogInformation("Query Expression Entrys");

            try
            {
                var aggregates = await _session.Get<ExpressionAggregate>(ExpressionsNamespaces.ExpressionAggregate);

                return new ExpressionResult { Entrys = aggregates.Expressions.ToArray()};
            }
            catch (AggregateNotFoundException)
            {
                return new ExpressionResult { Entrys = new ExpressionEntry[0] };
            }
        }
    }
}