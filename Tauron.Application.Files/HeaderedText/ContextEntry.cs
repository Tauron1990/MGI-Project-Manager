using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class ContextEntry
    {
        internal ContextEntry(string key, string content)
        {
            Key = Argument.NotNull(key, nameof(key));
            Content = content;
        }

        public string Key { get; }
        public string Content { get; }
    }
}