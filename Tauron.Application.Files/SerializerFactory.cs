using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Fluent.Impl;
using Tauron.Application.Files.Serialization.Core.Impl;

namespace Tauron.Application.Files
{
    [PublicAPI]
    public static class SerializerFactory
    {
        public static IBinaryConfiguration CreateBinary()
        {
            return new BinarySerializerConfiguration();
        }

        public static IIniSerializerConfiguration CreateIni<TType>()
        {
            return new IniConfiguration(typeof(TType));
        }

        public static IXmlSerializerConfiguration CreateXml<TType>(string rootName, XDeclaration? xDeclaration, [NotNull] XNamespace rootNamespace)
        {
            return new XmlSerializerConfiguration(rootName, xDeclaration, rootNamespace, typeof(TType));
        }

        public static IXmlSerializerConfiguration CreateXml<TType>(string rootName, XDeclaration? xDeclaration)
        {
            return CreateXml<TType>(rootName, xDeclaration, XNamespace.None);
        }

        public static IXmlSerializerConfiguration CreateXml<TType>(string rootName)
        {
            return CreateXml<TType>(rootName, null);
        }

        public static IXmlSerializerConfiguration CreateXml<TType>()
        {
            return CreateXml<TType>("Root");
        }

        public static IHeaderedFileSerializerConfiguration CreateHeaderedFile<TType>()
        {
            return new HeaderedFileSerializerConfiguration(typeof(TType));
        }
    }
}