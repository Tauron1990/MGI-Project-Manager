﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ServiceManager.Core
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class ProgressBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Hidden;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}