using JetBrains.Annotations;
using Tauron.Application.Files.Ini;
using Tauron.Application.Files.Ini.Parser;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini
{
    internal class IniContext : ContextImplBase
    {
        public IniContext([NotNull] SerializationContext context)
            : base(context)
        {
            File = context.SerializerMode == SerializerMode.Deserialize
                ? IniFile.Parse(TextReader)
                : new IniFile();
        }

        public IniFile File { get; }

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode != SerializerMode.Serialize)
            {
                base.Dispose(disposing);
                return;
            }

            new IniWriter(File, TextWriter).Write();

            base.Dispose(disposing);
        }
    }
}