namespace Tauron.Application.OptionsStore.Data
{
    public struct OptionsPair
    {
        public string Value { get; }

        public string Key { get;  }

        public OptionsPair(string value, string key)
        {
            Value = value;
            Key = key;
        }
    }
}