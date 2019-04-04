using System;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface ITimeTask
    {
        string Name { get; }

        TimeSpan Interval { get; }

        Task TriggerAsync();
    }
}