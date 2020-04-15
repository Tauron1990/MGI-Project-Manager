using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class StringCnverter : SimpleConverter<string>
    {
        public override object ConvertBack(string target)
        {
            return target;
        }

        public override string Convert(object? source)
        {
            return source?.ToString() ?? string.Empty;
        }
    }
}