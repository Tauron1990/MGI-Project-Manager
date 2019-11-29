using System.Windows.Data;
using System.Windows.Markup;

namespace Tauron.Application.Wpf.Converter
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public sealed class NullableLongToIntConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create() => new Converter();

        private class Converter : ValueConverterBase<int, long?>
        {
            protected override bool CanConvertBack => true;

            protected override long? Convert(int value) => value;

            protected override int ConvertBack(long? value) => value == null ? 0 : (int)value.Value;
        }
    }
}