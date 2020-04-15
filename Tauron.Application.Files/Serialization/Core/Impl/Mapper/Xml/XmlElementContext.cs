using System;
using System.Xml;
using System.Xml.Linq;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlElementContext : ContextImplBase
    {
        private XContainer? _currentElement;

        public XmlElementContext(SerializationContext original, XDeclaration? declaration,
            XNamespace? xNamespace, string rootName) : base(original)
        {
            XDocument? doc = null;
            XElement? ele = null;

            switch (Argument.NotNull(original, nameof(original)).SerializerMode)
            {
                case SerializerMode.Deserialize:
                    doc = XDocument.Load(TextReader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
                    break;
                case SerializerMode.Serialize:
                    if (declaration != null)
                    {
                        doc = new XDocument(declaration, new XElement(xNamespace + rootName));
                        _currentElement = doc;
                    }
                    else
                    {
                        ele = new XElement(rootName);
                        _currentElement = ele;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(original));
            }

            XElement = (ele ?? doc?.Root) ?? throw new InvalidOperationException("XElement is Null");
        }

        public XElement XElement { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (Original.SerializerMode != SerializerMode.Serialize) return;

            using (var writer = XmlWriter.Create(TextWriter, new XmlWriterSettings {Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates}))
            {
                _currentElement?.WriteTo(writer);
                _currentElement = null;
            }

            base.Dispose(disposing);
        }
    }
}