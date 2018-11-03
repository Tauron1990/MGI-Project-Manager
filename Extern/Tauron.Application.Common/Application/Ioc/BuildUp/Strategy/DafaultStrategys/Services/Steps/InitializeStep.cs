using System;
using System.Linq;
using System.Reflection;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public sealed class InitializeStep : InjectorStep
    {
        public override string ErrorMessage { get; } = nameof(InitializeStep);

        public override StepId Id => StepIds.Initialize;

        public override StepId OnExecute(InjectorContext context)
        {
            if (context.BuildParameters != null)
                foreach (var export in from buildParameter in context.BuildParameters
                    where buildParameter != null
                    select buildParameter.CreateExport()
                    into export
                    where export != null
                    select export)
                    context.ReflectionContext.BuildParametersRegistry.Register(export, int.MaxValue);

            if (context.ImportInterceptor != null)
                context.ReflectionContext.InterceptorCallback =
                    new ImportInterceptorHelper(context.ImportInterceptor, (MemberInfo) context.MemberInfo,
                        context.Metadata, context.Target).Intercept;

            if (context.Metadata.Metadata.TryGetValue(LevelSpecificImport.LevelMetadata, out var val))
                try
                {
                    context.ReflectionContext.Level = (int) val;
                }
                catch (InvalidCastException)
                {
                    context.ReflectionContext.Level = int.MaxValue;
                }
            else context.ReflectionContext.Level = int.MaxValue;

            return base.OnExecute(context);
        }
    }
}