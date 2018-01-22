namespace Tauron.Application.SimpleWorkflow
{
    public interface IStep<in TContext>
    {
        StepId Id { get; }

        StepId OnExecute(TContext context);

        StepId NextElement(TContext context);

        void OnExecuteFinish(TContext context);
    }
}