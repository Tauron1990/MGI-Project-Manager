using System;
using System.Threading.Tasks;
using Tauron.Application.OptionsStore.Data;

namespace Tauron.Application.OptionsStore.Store
{
    public sealed class OptionImpl : IOption
    {
        private readonly Func<OptionsPair, Task> _update;
        private readonly Action<OptionsPair> _updateSync;

        public OptionImpl(string key, string value, Func<OptionsPair, Task> update, Action<OptionsPair> updateSync)
        {
            _update = update;
            _updateSync = updateSync;
            Key = key;
            Value = value;
        }

        public static IOption EmptyOption { get; } = new Empty();

        public string Key { get; }
        public string Value { get; private set; }

        public async Task SetValueAsync(string value)
        {
            await _update(new OptionsPair(value, Key));
            Value = value;
        }

        public void SetValue(string value)
        {
            _updateSync(new OptionsPair(value, Key));
        }

        private sealed class Empty : IOption
        {
            public string Key { get; } = string.Empty;
            public string Value { get; } = string.Empty;

            public Task SetValueAsync(string value) => Task.CompletedTask;

            public void SetValue(string value)
            {
            }
        }
    }
}