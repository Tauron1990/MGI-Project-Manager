using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal sealed class InternalXmlSerializer : SerializerBase<XmlElementContext>
    {
        private readonly XDeclaration _declaration;
        private readonly string _rootName;
        private readonly XNamespace _xNamespace;

        public InternalXmlSerializer([NotNull] ObjectBuilder builder, [NotNull] SimpleMapper<XmlElementContext> mapper, [NotNull] string rootName, 
            [CanBeNull] XDeclaration declaration, [CanBeNull] XNamespace xNamespace)
            : base(builder, mapper, ContextMode.Text)
        {
            _rootName = Argument.NotNull(rootName, nameof(rootName));
            _declaration = declaration;
            _xNamespace = xNamespace;
        }

        public override XmlElementContext BuildContext(SerializationContext context) 
            => new XmlElementContext(Argument.NotNull(context, nameof(context)), _declaration, _xNamespace, _rootName);

        public override void CleanUp(XmlElementContext context)
        {
            Argument.NotNull(context, nameof(context)).Dispose();
        }
    }
}