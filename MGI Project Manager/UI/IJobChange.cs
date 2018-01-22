namespace Tauron.Application.MgiProjectManager.UI
{
    public interface IJobChange
    {
        string CustomLabel { get; }
        void   OnJobChange(JobItem item);

        bool FinalizeNext();

        bool? CanNext();
    }
}