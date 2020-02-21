using System;
using System.Globalization;
using System.Windows.Data;
using Catel.MVVM;
using JetBrains.Annotations;
using Tauron.Application.Wpf.Converter;

namespace Tauron.Application.Wpf.UI
{
    public sealed class ViewModelConverterExtension : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new ViewModelConverter();
        }
    }

    public class ViewModelConverter : IValueConverter
    {
        public object Convert(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
        {
            if (!(value is ViewModelBase model)) return value;

            var manager = AutoViewLocation.Manager;

            return manager.ResolveView(model) ?? value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FrameworkObject(value).DataContext;
        }
    }
}