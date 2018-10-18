using System.Threading.Tasks;

namespace Tauron.ServiceBootstrapper.Jobs
{
    public interface IJob
    {
        Task Invoke(JobContext context);
    }
}