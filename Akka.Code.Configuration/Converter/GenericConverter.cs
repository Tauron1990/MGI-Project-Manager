namespace Akka.Code.Configuration.Converter
{
    public sealed class GenericConverter : ConverterBase
    {
        public static readonly GenericConverter Converter = new GenericConverter();

        public override string? ToElementValue(object? obj) => obj?.ToString();
    }
}