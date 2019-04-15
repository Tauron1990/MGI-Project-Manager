using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Data
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
}