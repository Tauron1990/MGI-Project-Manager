using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public abstract class Injectorbase<TMember> : MemberInjector
    {
        protected Injectorbase([NotNull] IMetadataFactory metadataFactory, [NotNull] TMember member, [NotNull] IResolverExtension[] resolverExtensions)
        {
            if (metadataFactory == null) throw new ArgumentNullException(nameof(metadataFactory));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (resolverExtensions == null) throw new ArgumentNullException(nameof(resolverExtensions));
            Member = member;

            // ReSharper disable once VirtualMemberCallInConstructor
            InjectorContext = new InjectorContext(metadataFactory, member, MemberType, resolverExtensions);
        }

        [NotNull]
// ReSharper disable once MemberCanBePrivate.Global
        protected InjectorContext InjectorContext { get; private set; }

        [NotNull]
        protected TMember Member { get; }

        [NotNull]
        protected abstract Type MemberType { get; }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual StepId InitializeMachine([NotNull] out ResolverFactory solidMachine)
        {
            solidMachine = ResolverFactory.DefaultResolverFactory;

            return ResolverFactory.StartId;
        }

        public override void Inject(object target, IContainer container, ImportMetadata metadata,
            IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters)
        {
            try
            {
                errorTracer.Phase = "Creating Resolver for " + target.GetType().Name + "(" + metadata + ")";

                InjectorContext.Tracer = errorTracer;
                InjectorContext.Metadata = metadata;
                InjectorContext.ImportInterceptor = interceptor;
                InjectorContext.BuildParameters = parameters;
                InjectorContext.Container = container;
                InjectorContext.Target = target;

                var start = InitializeMachine(out var fac);

                InjectorContext.Machine = fac;

                fac.Begin(start, InjectorContext);

                if (InjectorContext.Resolver == null) throw new InvalidOperationException("No Resolver Created");

                var value = InjectorContext.Resolver.Create(errorTracer);

                if (errorTracer.Exceptional) return;

                Inject(target, value);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }

        protected abstract void Inject([NotNull] object target, [CanBeNull] object value);
    }
}