using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class HeaderedFile
    {
        public HeaderedFile([NotNull] FileDescription description)
        {
            Context = new FileContext(description);
        }

        internal HeaderedFileWriter? CurrentWriter { get; set; }

        [NotNull] public FileContext Context { get; private set; }

        [CanBeNull] public string Content { get; internal set; }

        public void Read([NotNull] TextReader reader)
        {
            var builder = new StringBuilder();

            Context.Reset();

            var compled = false;

            foreach (var textLine in reader.EnumerateTextLines())
            {
                if (compled)
                {
                    builder.AppendLine(textLine);
                    continue;
                }

                var textLineTemp = textLine.Trim();
                var temp = textLineTemp.Split(new[] {' '}, 2);
                if (Context.IsKeyword(temp[0]))
                {
                    var key = temp[0];
                    var content = string.Empty;

                    if (temp.Length < 1) content = temp[1];

                    Context.Add(new ContextEnry(key, content));
                }
                else
                {
                    builder.AppendLine(textLine);
                    compled = true;
                }
            }
        }

        [NotNull]
        public HeaderedFileWriter CreateWriter()
        {
            return CurrentWriter ?? new HeaderedFileWriter(this);
        }
    }
}