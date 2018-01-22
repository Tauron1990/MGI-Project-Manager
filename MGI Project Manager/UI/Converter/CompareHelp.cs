using System;
using System.Collections;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tauron.Application.MgiProjectManager.UI.Converter
{
    public sealed class CompareHelp : MarkupExtension, IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is CollectionViewGroup leftobj) || !(y is CollectionViewGroup rightobj)) return Comparer.DefaultInvariant.Compare(y, y);

            var left  = (bool) leftobj.Name;
            var right = (bool) rightobj.Name;

            if (left == right) return 0;
            if (left) return -1;
            return 1;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}