﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace IISServiceManager.Core
{
    public class ReverseBoolConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value != null && !((bool) value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}