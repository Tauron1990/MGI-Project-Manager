using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.OptionsStore
{
    [PublicAPI]
    public interface IAppOptions
    {
        string Name { get; }

        Task<IOption> GetOptionAsync(string name);

        Task DeleteOptionAsync(string name);

        IOption GetOption(string name);

        void DeleteOption(string name);
    }
}