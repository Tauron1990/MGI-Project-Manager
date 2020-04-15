using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class GenericEnumConverter : SimpleConverter<string>
    {
        private readonly Type _enumType;

        public GenericEnumConverter([NotNull] Type enumType)
        {
            if (enumType.BaseType != typeof(Enum)) throw new SerializerElementException("The Type is no Enum");

            _enumType = enumType;
        }

        public override object ConvertBack([NotNull] string target) => Enum.Parse(_enumType, target);

        [NotNull]
        public override string Convert(object source) => source == null ? string.Empty : source.ToString();
    }
}