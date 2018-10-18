using System;
using System.Threading.Tasks;

namespace Tauron.ServiceBootstrapper.Jobs
{
    public interface IJobManager : IDisposable
    {
        void Start();

        void Stop();
    }
}