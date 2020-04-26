namespace Akka.Code.Configuration.Converter
{
    public abstract class TypedConverter<TType> : ConverterBase
    {
        public override string? ToElementValue(object? obj)
        {
            if (obj is TType value)
                return ConvertGeneric(value);
            return null;
        }

        protected abstract string? ConvertGeneric(TType obj);
    }
}