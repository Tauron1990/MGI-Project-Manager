using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ServiceManager.Services;

namespace ServiceManager.Core
{
    [ValueConversion(typeof(ServiceStade), typeof(Brush))]
    public sealed class ServiceStadeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServiceStade serviceStade)
            {
                switch (serviceStade)
                {
                    case ServiceStade.Error:
                        return Brushes.Red;
                    case ServiceStade.Running:
                        return Brushes.Green;
                    case ServiceStade.Ready:
                        return Brushes.Yellow;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => null;
    }
}