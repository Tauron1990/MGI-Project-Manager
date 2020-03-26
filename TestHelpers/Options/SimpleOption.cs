using System.Threading.Tasks;
using Tauron.Application.OptionsStore;
using Xunit;

namespace TestHelpers.Options
{
    public sealed class SimpleOption : IOption
    {
        private readonly bool _isReadonly;

        public SimpleOption(string key, string value, bool isReadonly)
        {
            _isReadonly = isReadonly;
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; private set; }

        public Task SetValueAsync(string value)
        {
            AssertReadonly();

            Value = value;

            return Task.CompletedTask;
        }

        public void SetValue(string value)
        {
            AssertReadonly();

            Value = value;
        }

        private void AssertReadonly()
            => Assert.False(_isReadonly, "Option is Set to Readonly");
    }
}