using System;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class GenericEnumConverter : SimpleConverter<string>
    {
        private readonly Type _enumType;

        public GenericEnumConverter(Type enumType)
        {
            if (enumType.BaseType != typeof(Enum)) throw new SerializerElementException("The Type is no Enum");

            _enumType = enumType;
        }

        public override object ConvertBack([NotNull] string target)
        {
            return Enum.Parse(_enumType, target);
        }

        public override string Convert(object? source)
        {
            return source == null ? string.Empty : source.ToString();
        }
    }
}