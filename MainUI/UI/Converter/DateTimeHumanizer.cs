using System;
using System.Windows.Data;
using Humanizer;
using Tauron.Application.Converter;

namespace Tauron.Application.MgiProjectManager.UI.Converter
{
    public sealed class DateTimeHumanizer : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        private class Converter : StringConverterBase<DateTime>
        {
            protected override string Convert(DateTime value)
            {
                return value.Humanize();
            }
        }
    }
}