using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class IsGenericStep : InjectorStep
    {
        public override string ErrorMessage { get; } = nameof(IsGenericStep);

        public override StepId Id => StepIds.GenericStep;

        public override StepId OnExecute(InjectorContext context)
        {
            context.ReflectionContext.AdditionalInfo = context.ReflectionContext.MemberType.GetGenericTypeDefinition();
            return base.OnExecute(context);
        }
    }
}