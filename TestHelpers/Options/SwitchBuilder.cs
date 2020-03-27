using System;
using System.Collections.Generic;

namespace TestHelpers.Options
{
    public sealed class SwitchBuilder<TEnum>
        where TEnum : Enum
    {
        private readonly ServicesConfiguration _configuration;

        private readonly Dictionary<TEnum, Action<ServicesConfiguration>> _switches = new Dictionary<TEnum, Action<ServicesConfiguration>>();
        private Action<ServicesConfiguration>? _generic;

        public SwitchBuilder(ServicesConfiguration configuration) 
            => _configuration = configuration;

        public SwitchBuilder<TEnum> Case(TEnum e, Action<ServicesConfiguration> config)
        {
            _switches[e] = config;
            return this;
        }

        public SwitchBuilder<TEnum> Default(Action<ServicesConfiguration> config)
        {
            _generic = config;
            return this;
        }

        public ServicesConfiguration Apply(TEnum e)
        {
            if (_switches.TryGetValue(e, out var a))
                a(_configuration);
            else
                _generic?.Invoke(_configuration);

            return _configuration;
        }
    }
}