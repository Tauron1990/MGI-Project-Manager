namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IOperationAction
    {
        string Name { get; }

        void Execude(Operation op);

        void Expired(Operation op);
    }
}