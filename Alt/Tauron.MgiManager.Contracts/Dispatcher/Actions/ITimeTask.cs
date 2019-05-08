using System;
using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Dispatcher.Actions
{
    public interface ITimeTask
    {
        string Name { get; }

        TimeSpan Interval { get; }

        Task TriggerAsync(IServiceProvider provider);
    }
}