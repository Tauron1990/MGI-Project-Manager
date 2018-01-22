using System;
using System.Collections.Generic;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ListResolverStep : ManyResolverStep
    {
        public override StepId Id => StepIds.ListResolver;

        protected override Type GetCurrentType(ReflectionContext context)
        {
            return context.CurrentType.GenericTypeArguments[0];
        }

        protected override IResolver CreateResolver(IEnumerable<IResolver> resolvers, Type listType)
        {
            return new ListResolver(resolvers, listType);
        }
    }
}