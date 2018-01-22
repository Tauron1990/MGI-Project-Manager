using System;

namespace Tauron.Application.MgiProjectManager.UI
{
    public sealed class JobItemStatusEventArgs : EventArgs
    {
        public JobItemStatusEventArgs(JobItem item)
        {
            Item = item;
        }

        public JobItem Item { get; }
    }
}