using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.OptionsStore
{
    [PublicAPI]
    public interface IAppOptions
    {
        string Name { get; }

        Task<IOption> GetOption(string name);

        Task DeleteOption(string name);
    }
}