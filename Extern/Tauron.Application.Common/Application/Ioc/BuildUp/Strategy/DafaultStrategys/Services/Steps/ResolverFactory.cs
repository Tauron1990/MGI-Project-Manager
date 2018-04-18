using JetBrains.Annotations;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ResolverFactory : Producer<InjectorStep, InjectorContext>
    {
        private static ResolverFactory _resolverFactory;

        public static StepId StartId => StepIds.Initialize;

        [NotNull]
        public static ResolverFactory DefaultResolverFactory
            => _resolverFactory ?? (_resolverFactory = CreateDefaultFactory());

        [NotNull]
        private static ResolverFactory CreateDefaultFactory()
        {
            var fac = new ResolverFactory();

            fac.SetStep(new InitializeStep())
               .AddCondition()
               .GoesTo(StepIds.ResolverCreation);
            fac.SetStep(new ResolverCreationStep())
               .AddCondition(
                             (con, sta) =>
                                 con.ReflectionContext.CurrentType.IsGenericType &&
                                 con.ReflectionContext.CurrentType.GetGenericTypeDefinition() == typeof(InstanceResolver<,>))
               .GoesTo(StepIds.SimpleResolver)
               //Lazy
               .AddCondition(
                             (con, step) =>
                                 con.ReflectionContext.CurrentType.IsGenericType &&
                                 (InjectorBaseConstants.Lazy == con.ReflectionContext.CurrentType.GetGenericTypeDefinition() ||
                                  InjectorBaseConstants.LazyWithMetadata ==
                                  con.ReflectionContext.CurrentType.GetGenericTypeDefinition()))
               .GoesTo(StepIds.LazyResolver)
               //Generic
               .AddCondition((con, sta) => con.ReflectionContext.CurrentType.IsGenericType
                                         &&
                                           con.ReflectionContext.CurrentType.GetGenericTypeDefinition() !=
                                           typeof(InstanceResolver<,>)
                                         &&
                                           con.ReflectionContext.CurrentType.GetGenericTypeDefinition() !=
                                           InjectorBaseConstants.Lazy
                                         &&
                                           con.ReflectionContext.CurrentType.GetGenericTypeDefinition() !=
                                           InjectorBaseConstants.LazyWithMetadata)
               .GoesTo(StepIds.GenericStep)
               //Array
               .AddCondition((con, sta) => con.ReflectionContext.CurrentType.IsArray)
               .GoesTo(StepIds.ArrayResolver)
               //Default
               .AddCondition()
               .GoesTo(StepIds.SimpleResolver);

            fac.SetStep(new SimpleResolverStep());
            fac.SetStep(new LazyResolverStep())
               .AddCondition()
               .GoesTo(StepIds.SimpleResolver);
            fac.SetStep(new ArrayResolverStep())
               .AddCondition()
               .GoesTo(StepIds.ResolverCreation);

            fac.SetStep(new IsGenericStep())
               //List
               .AddCondition(
                             (con, step) =>
                                 con.ReflectionContext.CurrentType.IsAssignableFrom(
                                                                                    InjectorBaseConstants.List.MakeGenericType(
                                                                                                                               con.ReflectionContext.CurrentType.GetGenericArguments()[0])))
               .GoesTo(StepIds.ListResolver)
               //Default
               .AddCondition()
               .GoesTo(StepIds.SimpleResolver);

            fac.SetStep(new ListResolverStep())
               .AddCondition()
               .GoesTo(StepIds.ResolverCreation);

            return fac;
        }
    }
}