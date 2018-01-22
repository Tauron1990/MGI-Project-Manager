using JetBrains.Annotations;

namespace Tauron.Application.SimpleWorkflow
{
    public interface ICondition<TContext>
    {
        StepId Select([NotNull] IStep<TContext> lastStep, TContext context);
    }
}