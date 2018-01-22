using System;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public sealed class SimpleResolverStep : InjectorStep
    {
        public override StepId Id => StepIds.SimpleResolver;

        public override StepId OnExecute(InjectorContext context)
        {
            var export = context.ReflectionContext.FindExport();
            var reflectionContext = context.ReflectionContext;

            if (export == null) return StepId.Invalid;

            var currentType = reflectionContext.CurrentType;
            var isExportFactory = currentType.IsGenericType &&
                                  currentType.GetGenericTypeDefinition() == typeof(InstanceResolver<,>);
            Type factoryType = null;

            if (isExportFactory)
            {
                factoryType = currentType.GenericTypeArguments[0];
                reflectionContext.MetadataType = currentType.GenericTypeArguments[1];
                reflectionContext.Metadata =
                    context.ReflectionContext.MetadataFactory.CreateMetadata(context.ReflectionContext.MetadataType,
                        export.Metadata);
            }

            context.Resolver = new SimpleResolver(export, context.Container, isExportFactory, factoryType,
                reflectionContext.Metadata,
                reflectionContext.MetadataType,
                reflectionContext.InterceptorCallback,
                currentType == typeof(ExportDescriptor), reflectionContext.ResolverExtensions);
            return base.OnExecute(context);
        }
    }
}