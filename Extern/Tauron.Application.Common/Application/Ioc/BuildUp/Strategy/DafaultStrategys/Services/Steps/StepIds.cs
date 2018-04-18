using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public static class StepIds
    {
        public static readonly StepId Initialize       = new StepId("Initialize");
        public static readonly StepId SimpleResolver   = new StepId("SimpleResolver");
        public static readonly StepId ArrayResolver    = new StepId("ArrayResolver");
        public static readonly StepId GenericStep      = new StepId("Generic");
        public static readonly StepId ResolverCreation = new StepId("ResolverCreation");
        public static readonly StepId ListResolver     = new StepId("ListResolver");
        public static readonly StepId LazyResolver     = new StepId("lazyResolver");
    }
}