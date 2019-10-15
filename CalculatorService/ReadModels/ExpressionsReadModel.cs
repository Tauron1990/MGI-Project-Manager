﻿using System.Collections.Generic;
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
    public sealed class ExpressionsReadModel : ReadModelBase<List<ExpressionEntry>, ExpressionsQuery>
    {
        private readonly ISession _session;
        private readonly ILogger<ExpressionsReadModel> _logger;

        public ExpressionsReadModel(IDispatcherClient client, ISession session, ILogger<ExpressionsReadModel> logger) 
            : base(client)
        {
            _session = session;
            _logger = logger;
        }

        protected override async Task<List<ExpressionEntry>> Query(ExpressionsQuery query)
        {
            _logger.LogInformation("Query Expression Entrys");

            try
            {
                var aggregates = await _session.Get<ExpressionAggregate>(ExpressionsNamespaces.ExpressionAggregate);

                return new List<ExpressionEntry>(aggregates.Expressions);
            }
            catch (AggregateNotFoundException)
            {
                return new List<ExpressionEntry>();
            }
        }
    }
}