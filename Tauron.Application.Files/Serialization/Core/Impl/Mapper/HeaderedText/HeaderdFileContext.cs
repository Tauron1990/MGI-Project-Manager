using JetBrains.Annotations;
using Tauron.Application.Files.HeaderedText;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText
{
    internal class HeaderdFileContext : ContextImplBase
    {
        private readonly FileDescription _description;
        private readonly HeaderedFile    _file;

        public HeaderdFileContext([NotNull] SerializationContext original, [NotNull] FileDescription description) : base(original)
        {
            Argument.NotNull(original, nameof(original));
            Argument.NotNull(description, nameof(description));

            _description = description;

            _file = new HeaderedFile(_description);
            if (original.SerializerMode == SerializerMode.Deserialize)
                _file.Read(TextReader);

            if (original.SerializerMode == SerializerMode.Serialize)
                _file.CreateWriter();
        }

        [NotNull]
        public string Content => _file.Content ?? string.Empty;

        [NotNull]
        public HeaderedFileWriter CurrentWriter => Argument.CheckResult(_file.CurrentWriter, "No Writer Found");

        [NotNull]
        public FileContext Context => _file.Context;

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode == SerializerMode.Deserialize)
            {
                base.Dispose(disposing);
                return;
            }

            var writer = _file.CurrentWriter;

// ReSharper disable once PossibleNullReferenceException
            writer.Save(TextWriter);

            base.Dispose(disposing);
        }
    }
}