using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlListElementConfiguration : IXmlListElementConfiguration, IXmlListAttributeConfiguration
    {
        private readonly IXmlSerializerConfiguration _config;
        private readonly SimpleMapper<XmlElementContext> _mapper;
        private readonly XmlElementTarget _parentTarget;
        private readonly XmlElementTarget _root;
        private readonly XmlElementTarget _target;
        private readonly Type _targeType;
        private SimpleConverter<string>? _converter;

        private string? _member;

        public XmlListElementConfiguration(IXmlSerializerConfiguration config, SimpleMapper<XmlElementContext> mapper, XmlElementTarget target,
            XmlElementTarget root, XmlElementTarget parentTarget, Type targeType)
        {
            _config = Argument.NotNull(config, nameof(config));
            _mapper = Argument.NotNull(mapper, nameof(mapper));
            _target = Argument.NotNull(target, nameof(target));
            _root = Argument.NotNull(root, nameof(root));
            _parentTarget = Argument.NotNull(parentTarget, nameof(parentTarget));
            _targeType = Argument.NotNull(targeType, nameof(targeType));
        }

        IXmlListAttributeConfiguration IXmlRootConfiguration<IXmlListAttributeConfiguration>.WithNamespace(XNamespace xNamespace)
        {
            WithNamespace(xNamespace);
            return this;
        }

        IXmlListAttributeConfiguration IWithMember<IXmlListAttributeConfiguration>.WithConverter(SimpleConverter<string> converter)
        {
            WithConverter(converter);
            return this;
        }

        IXmlListAttributeConfiguration IWithMember<IXmlListAttributeConfiguration>.WithMember(string name)
        {
            WithMember(Argument.NotNull(name, nameof(name)));
            return this;
        }

        public IXmlSerializerConfiguration Apply()
        {
            _member ??= _target.Name;

            var map = new XmlListMapper(_member, _targeType, _parentTarget, _root, _converter);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlListElementConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IXmlListElementConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IXmlListElementConfiguration Element(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Element, Name = Argument.NotNull(name, nameof(name))};
            _target.SubElement = target;

            return new XmlListElementConfiguration(_config, _mapper, target, _root, _parentTarget, _targeType);
        }

        public IXmlListElementConfiguration WithNamespace(XNamespace xNamespace)
        {
            _target.XNamespace = xNamespace;
            return this;
        }

        public IXmlSerializerConfiguration WithSubSerializer<TSerisalize>(ISerializer serializer)
        {
            _member ??= _target.Name;

            var mapper = new XmlListSubSerializerMapper(_member, typeof(TSerisalize), _parentTarget, _root, serializer as ISubSerializer);
            _mapper.Entries.Add(mapper);

            return _config;
        }

        public IXmlListAttributeConfiguration Attribute(string name)
        {
            var target = new XmlElementTarget {TargetType = XmlElementTargetType.Attribute, Name = Argument.NotNull(name, nameof(name))};
            _target.SubElement = target;

            return new XmlListElementConfiguration(_config, _mapper, target, _root, _parentTarget, _targeType);
        }
    }
}