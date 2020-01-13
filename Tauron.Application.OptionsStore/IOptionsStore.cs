using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.OptionsStore
{
    [PublicAPI]
    public interface IOptionsStore
    {
        IAppOptions GetAppOptions(string applicationName);
    }
}