using System;
using System.Threading.Tasks;
using Tauron.Application.OptionsStore.Data;

namespace Tauron.Application.OptionsStore.Store
{
    public class OptionImpl : IOption 
    {
        private readonly Func<OptionsPair, Task> _update;
        public string Key { get; }
        public string Value { get; private set; }

        public OptionImpl(string key, string value, Func<OptionsPair, Task> update)
        {
            _update = update;
            Key = key;
            Value = value;
        }
        
        public async Task SetValue(string value)
        {
            await _update(new OptionsPair(Key, value));
            Value = value;
        }
    }
}