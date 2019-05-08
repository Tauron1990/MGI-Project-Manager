using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface ILoggingDb
    {
        Task LimitCount(int count);
    }
}