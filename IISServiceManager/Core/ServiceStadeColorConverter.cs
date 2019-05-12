using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using IISServiceManager.Contratcs;

namespace IISServiceManager.Core
{
    public class ServiceStadeColorConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ServiceStade serviceStade)) return Colors.DarkViolet;

            switch (serviceStade)
            {
                case ServiceStade.Running:
                    return Colors.Green;
                case ServiceStade.Stopped:
                    return Colors.Red;
                case ServiceStade.NotInstalled:
                    return Colors.Blue;
                default:
                    return Colors.DarkViolet;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}