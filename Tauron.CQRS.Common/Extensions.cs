using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Tauron.CQRS.Common
{
    public static class Extensions
    {
        public static bool TryGetTypedValue<TKey, TValue, TActual>(
            this IDictionary<TKey, TValue> data,
            TKey key,
            out TActual value, Func<TValue, TActual>? converter = null) where TActual : TValue
        {
            if (data.TryGetValue(key, out var tmp))
            {
                if (converter != null)
                {
                    value = converter(tmp);
                    return true;
                }
                if (tmp is TActual actual)
                {
                    value = actual;
                    return true;
                }
                value = default;
                return false;
            }
            value = default;
            return false;
        }
    }
}