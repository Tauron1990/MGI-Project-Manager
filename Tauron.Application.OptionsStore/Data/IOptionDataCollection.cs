using System.Threading.Tasks;

namespace Tauron.Application.OptionsStore.Data
{
    public interface IOptionDataCollection
    {
        Task<OptionsPair> GetOptionAsync(string key);

        Task DeleteOptionAsync(string key);

        Task UpdateAsync(OptionsPair pair);

        OptionsPair GetOption(string key);

        void DeleteOption(string key);

        void Update(OptionsPair pair);
    }
}