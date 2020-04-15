using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class StringCnverter : SimpleConverter<string>
    {
        public override object ConvertBack([NotNull] string target) => target;

        [NotNull]
        public override string Convert(object source) => source?.ToString() ?? string.Empty;
    }
}