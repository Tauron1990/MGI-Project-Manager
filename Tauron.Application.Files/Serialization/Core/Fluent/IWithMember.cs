using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Fluent
{
    public interface IWithMember<out TConfig>
    {
        TConfig WithMember(string name);

        TConfig WithConverter(SimpleConverter<string> converter);
    }
}