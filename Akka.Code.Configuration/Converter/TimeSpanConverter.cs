using System;

namespace Akka.Code.Configuration.Converter
{
    public sealed class TimeSpanConverter : TypedConverter<TimeSpan>
    {
        public static readonly TimeSpanConverter Converter = new TimeSpanConverter();

        protected override string? ConvertGeneric(TimeSpan obj) 
            => obj.Ticks == -1 ? "infinite" : $"{obj.TotalMilliseconds:F0}ms";
    }
}