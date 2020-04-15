using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal sealed class InternalXmlSerializer : SerializerBase<XmlElementContext>
    {
        private readonly XDeclaration? _declaration;
        private readonly string _rootName;
        private readonly XNamespace? _xNamespace;

        public InternalXmlSerializer(ObjectBuilder builder, SimpleMapper<XmlElementContext> mapper, string rootName,
            XDeclaration? declaration, XNamespace? xNamespace)
            : base(builder, mapper, ContextMode.Text)
        {
            _rootName = Argument.NotNull(rootName, nameof(rootName));
            _declaration = declaration;
            _xNamespace = xNamespace;
        }

        public override XmlElementContext BuildContext(SerializationContext context)
        {
            return new XmlElementContext(Argument.NotNull(context, nameof(context)), _declaration, _xNamespace, _rootName);
        }

        public override void CleanUp(XmlElementContext context)
        {
            Argument.NotNull(context, nameof(context)).Dispose();
        }
    }
}