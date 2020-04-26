using Akka.Code.Configuration.Elements;

namespace Akka.Code.Configuration.Converter
{
    public sealed class EnumConverter : ConverterBase
    {
        public static readonly EnumConverter Converter = new EnumConverter();

        public override string? ToElementValue(object? obj)
        {
            var str = obj?.ToString();
            if (str == null || str == "NotSet")
                return null;

            return str;
        }
    }
}