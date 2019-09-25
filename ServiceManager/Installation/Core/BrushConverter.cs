using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ServiceManager.Installation.Core
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool temp && temp)
                return new LinearGradientBrush(Colors.LightGray, Colors.White, new Point(0, 0), new Point(1, 1));

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}