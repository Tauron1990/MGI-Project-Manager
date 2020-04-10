namespace Tauron.Application.Workflow
{
    public interface IStep<in TContext>
    {
        string ErrorMessage { get; }

        StepId Id { get; }

        StepId OnExecute(TContext context);

        StepId NextElement(TContext context);

        void OnExecuteFinish(TContext context);
    }
}