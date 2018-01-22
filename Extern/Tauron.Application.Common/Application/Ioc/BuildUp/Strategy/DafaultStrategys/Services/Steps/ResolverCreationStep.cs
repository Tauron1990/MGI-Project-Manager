using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ResolverCreationStep : InjectorStep
    {
        public override StepId Id => StepIds.ResolverCreation;
    }
}