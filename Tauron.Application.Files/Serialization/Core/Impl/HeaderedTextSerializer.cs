using JetBrains.Annotations;
using Tauron.Application.Files.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.HeaderedText;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class HeaderedTextSerializer : SerializerBase<HeaderdFileContext>
    {
        private readonly FileDescription _description;

        public HeaderedTextSerializer(ObjectBuilder builder, SimpleMapper<HeaderdFileContext> mapper, [NotNull] FileDescription description)
            : base(builder, mapper, ContextMode.Text)
        {
            _description = Argument.NotNull(description, nameof(description));
        }

        public override HeaderdFileContext BuildContext(SerializationContext context)
        {
            return new HeaderdFileContext(context, _description);
        }

        public override void CleanUp(HeaderdFileContext context)
        {
            Argument.NotNull(context, nameof(context)).Dispose();
        }
    }
}