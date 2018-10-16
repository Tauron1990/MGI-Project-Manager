using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class InjectorContext
    {
        public InjectorContext([NotNull] IMetadataFactory metadataFactory, [NotNull] object memberInfo,
            [NotNull] Type memberType, [CanBeNull] IResolverExtension[] resolverExtensions)
        {
            ReflectionContext = new ReflectionContext(metadataFactory, memberType, this,
                resolverExtensions ?? new IResolverExtension[0]);
            MemberInfo = memberInfo;
        }

        [NotNull] public ReflectionContext ReflectionContext { get; }

        [CanBeNull] public BuildParameter[] BuildParameters { get; set; }

        [NotNull] public object MemberInfo { get; }

        [NotNull] public ErrorTracer Tracer { get; set; }

        [NotNull] public ImportMetadata Metadata { get; set; }

        [CanBeNull] public IResolver Resolver { get; set; }

        [NotNull] public ResolverFactory Machine { get; set; }

        [CanBeNull] public IImportInterceptor ImportInterceptor { get; set; }

        [NotNull] public IContainer Container { get; set; }

        [NotNull] public object Target { get; set; }
    }
}