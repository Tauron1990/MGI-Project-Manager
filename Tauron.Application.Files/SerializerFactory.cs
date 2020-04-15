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
        [NotNull]
        public static IBinaryConfiguration CreateBinary() => new BinarySerializerConfiguration();

        [NotNull]
        public static IIniSerializerConfiguration CreateIni<TType>() => new IniConfiguration(typeof(TType));

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName, [CanBeNull] XDeclaration xDeclaration, [NotNull] XNamespace rootNamespace) => new XmlSerializerConfiguration(rootName, xDeclaration, rootNamespace, typeof(TType));

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName, [CanBeNull] XDeclaration xDeclaration) => CreateXml<TType>(rootName, xDeclaration, XNamespace.None);

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>([NotNull] string rootName) => CreateXml<TType>(rootName, null);

        [NotNull]
        public static IXmlSerializerConfiguration CreateXml<TType>() => CreateXml<TType>("Root");

        [NotNull]
        public static IHeaderedFileSerializerConfiguration CreateHeaderedFile<TType>() => new HeaderedFileSerializerConfiguration(typeof(TType));
    }
}