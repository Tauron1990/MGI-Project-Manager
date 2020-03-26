using System;
using TestHelpers.Options;

namespace TestHelpers
{
    public static class TestHelperExtensions
    {
        public static SwitchBuilder<TEnum> Switch<TEnum>(this ServicesConfiguration config) 
            where TEnum : Enum
        {
            return new SwitchBuilder<TEnum>(config);
        }
    }
}