using System;
using System.Collections.Generic;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ArrayResolverStep : ManyResolverStep
    {
        public override StepId Id => StepIds.ArrayResolver;

        protected override Type GetCurrentType(ReflectionContext context)
        {
            return context.CurrentType.GetElementType();
        }

        protected override IResolver CreateResolver(IEnumerable<IResolver> resolvers, Type listType)
        {
            return new ArrayResolver(resolvers, listType.GetElementType());
        }
    }
}