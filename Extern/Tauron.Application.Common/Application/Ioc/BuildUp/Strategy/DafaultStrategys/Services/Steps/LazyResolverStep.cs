using System;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public sealed class LazyResolverStep : InjectorStep
    {
        private Type _lazyType;

        public override StepId Id => StepIds.LazyResolver;

        public override StepId OnExecute(InjectorContext context)
        {
            _lazyType = context.ReflectionContext.CurrentType;

            context.ReflectionContext.CurrentType = _lazyType.GenericTypeArguments[0];

            return StepId.None;
        }

        public override void OnExecuteFinish(InjectorContext context)
        {
            var resolver = context.Resolver as SimpleResolver;
            if (resolver == null)
                return;

            context.Resolver = new LazyResolver(resolver, _lazyType, context.ReflectionContext.MetadataFactory);

            base.OnExecuteFinish(context);
        }
    }
}