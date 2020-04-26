using System;

namespace Akka.Code.Configuration.Converter
{
    public abstract class ConverterBase
    {
        public static ConverterBase Find(Type type)
        {
            if (type == typeof(TimeSpan))
                return TimeSpanConverter.Converter;
            if (type.BaseType == typeof(Enum))
                return EnumConverter.Converter;
            else
                return GenericConverter.Converter;
        }

        public abstract string? ToElementValue(object? obj);
    }
}