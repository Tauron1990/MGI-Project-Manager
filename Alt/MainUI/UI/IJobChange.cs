using System;

namespace Tauron.Application.MgiProjectManager.UI
{
    public interface IJobChange
    {
        event EventHandler ConnectFailed;

        string CustomLabel { get; }
        void   OnJobChange(JobItem item);

        bool FinalizeNext();

        bool? CanNext();
    }
}