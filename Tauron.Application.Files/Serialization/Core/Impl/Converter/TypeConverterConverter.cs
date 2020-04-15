using System.ComponentModel;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class TypeConverterConverter : SimpleConverter<string>
    {
        private readonly TypeConverter _converter;

        public TypeConverterConverter([NotNull] TypeConverter converter) => _converter = Argument.NotNull(converter, nameof(converter));

        public override object ConvertBack([NotNull] string target) => _converter.ConvertFromString(target);

        [NotNull]
        public override string Convert(object source) => _converter.ConvertToString(Argument.NotNull(source, nameof(source))) ?? string.Empty;
    }
}