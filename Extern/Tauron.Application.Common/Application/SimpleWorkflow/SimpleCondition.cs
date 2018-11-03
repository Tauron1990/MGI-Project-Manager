using System;
using JetBrains.Annotations;

namespace Tauron.Application.SimpleWorkflow
{
    [PublicAPI]
    public class SimpleCondition<TContext> : ICondition<TContext>
    {
        public SimpleCondition()
        {
            Target = StepId.None;
        }

        [CanBeNull] public Func<TContext, IStep<TContext>, bool> Guard { get; set; }

        public StepId Target { get; set; }

        public StepId Select(IStep<TContext> lastStep, TContext context)
        {
            if (Guard == null || Guard(context, lastStep)) return Target;

            return StepId.None;
        }

        public override string ToString()
        {
            return "Target: " + Target;
        }
    }
}