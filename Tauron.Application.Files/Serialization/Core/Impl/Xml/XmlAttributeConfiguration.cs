using System;
using System.Xml.Linq;
using Tauron.Application.Files.Serialization.Core.Fluent;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class XmlAttributeConfiguration : IXmlAttributConfiguration
    {
        private readonly IXmlSerializerConfiguration _config;
        private readonly SimpleMapper<XmlElementContext> _mapper;
        private readonly XmlElementTarget _rootTarget;
        private readonly XmlElementTarget _target;
        private readonly Type _targetType;
        private SimpleConverter<string>? _converter;

        private string? _member;

        public XmlAttributeConfiguration(IXmlSerializerConfiguration config, XmlElementTarget rootTarget, XmlElementTarget target,
            SimpleMapper<XmlElementContext> mapper, Type targetType)
        {
            _config = Argument.NotNull(config, nameof(config));
            _rootTarget = Argument.NotNull(rootTarget, nameof(rootTarget));
            _target = Argument.NotNull(target, nameof(target));
            _mapper = Argument.NotNull(mapper, nameof(mapper));
            _targetType = Argument.NotNull(targetType, nameof(targetType));
        }

        public IXmlSerializerConfiguration Apply()
        {
            if (_member == null && _target != null) _member = _target.Name;

            var map = new XmlMapper(_member, _targetType, _converter, _rootTarget);
            _mapper.Entries.Add(map);

            return _config;
        }

        public IXmlAttributConfiguration WithMember(string member)
        {
            _member = member;
            return this;
        }

        public IXmlAttributConfiguration WithConverter(SimpleConverter<string> converter)
        {
            _converter = converter;
            return this;
        }

        public IXmlAttributConfiguration WithNamespace(XNamespace xNamespace)
        {
            _target.XNamespace = xNamespace;
            return this;
        }
    }
}