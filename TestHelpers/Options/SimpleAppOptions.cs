using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.OptionsStore;
using Xunit;

namespace TestHelpers.Options
{
    public sealed class SimpleAppOptions : IAppOptions
    {
        private readonly bool _isReadOnly;
        private readonly Dictionary<string, IOption> _options;

        public SimpleAppOptions(string name, bool isReadOnly = false, params (string key, IOption option)[] options)
        {
            _isReadOnly = isReadOnly;
            Name = name;

            _options = new Dictionary<string, IOption>();

            foreach (var (key, option) in options)
                _options[key] = option;
        }

        public string Name { get; }

        public Task<IOption> GetOptionAsync(string name)
        {
            try
            {
                return Task.FromResult(GetOption(name));
            }
            catch (Exception e)
            {
                return Task.FromException<IOption>(e);
            }
        }

        public Task DeleteOptionAsync(string name)
        {
            DeleteOption(name);

            return Task.CompletedTask;
        }

        public IOption GetOption(string name)
        {
            if (_options.TryGetValue(name, out var value))
                return value;

            AssertIsReadOnly();

            value = new SimpleOption(name, string.Empty, false);
            _options[name] = value;
            return value;
        }

        public void DeleteOption(string name)
        {
            AssertIsReadOnly();

            _options.Remove(name);
        }

        private void AssertIsReadOnly()
            => Assert.False(_isReadOnly, "Not Option can be Added");
    }
}