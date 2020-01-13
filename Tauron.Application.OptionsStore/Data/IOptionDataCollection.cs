using System.Threading.Tasks;

namespace Tauron.Application.OptionsStore.Data
{
    public interface IOptionDataCollection
    {
        Task<OptionsPair> GetOption(string key);

        Task DeleteOption(string key);

        Task Update(OptionsPair pair);
    }
}