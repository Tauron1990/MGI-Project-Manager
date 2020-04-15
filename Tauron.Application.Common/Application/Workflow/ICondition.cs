using JetBrains.Annotations;

namespace Tauron.Application.Workflow
{
    public interface ICondition<TContext>
    {
        StepId Select([NotNull] IStep<TContext> lastStep, TContext context);
    }
}