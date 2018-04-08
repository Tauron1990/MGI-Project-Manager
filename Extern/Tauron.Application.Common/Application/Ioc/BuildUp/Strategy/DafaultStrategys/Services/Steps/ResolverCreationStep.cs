using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ResolverCreationStep : InjectorStep
    {
        public override string ErrorMessage { get; } = nameof(ResolverCreationStep);

        public override StepId Id => StepIds.ResolverCreation;
    }
}