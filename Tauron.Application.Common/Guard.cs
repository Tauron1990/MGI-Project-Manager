using System;

namespace Tauron
{
    public static class Guard
    {
        public static TType CheckNull<TType>(TType? toCkeck, string name)
            where TType : class
        {
            if (toCkeck == null)
                throw new ArgumentNullException(name ?? nameof(toCkeck));

            return toCkeck;
        }
    }
}