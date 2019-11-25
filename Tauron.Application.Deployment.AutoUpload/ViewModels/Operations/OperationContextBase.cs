namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationContextBase
    {
        class DummyContext : OperationContextBase
        {
            
        }

        public static readonly OperationContextBase Empty = new DummyContext();

        public Redirection? Redirection { get; set; }
    }
}