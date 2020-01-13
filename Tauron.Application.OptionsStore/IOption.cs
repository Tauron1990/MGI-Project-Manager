using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.OptionsStore
{
    [PublicAPI]
    public interface IOption
    {
        string Key { get; }

        string Value { get; }

        Task SetValue(string value);
    }
}