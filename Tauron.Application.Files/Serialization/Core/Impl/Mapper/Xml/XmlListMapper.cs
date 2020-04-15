using System;
using System.Linq;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlListMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly ListBuilder? _listBuilder;
        private readonly XmlElementTarget _rootTarget;
        private readonly XmlElementSerializer _serializer;

        public XmlListMapper(string? membername, Type targetType, XmlElementTarget rootTarget, XmlElementTarget target, SimpleConverter<string>? converter)
            : base(membername, targetType)
        {
            _rootTarget = rootTarget;

            if (MemberType != null) _listBuilder = new ListBuilder(MemberType);

            if (converter == null && _listBuilder != null) converter = ConverterFactory.CreateConverter(TargetMember, _listBuilder.ElemenType);

            _serializer = new XmlElementSerializer(target, converter);
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            _listBuilder?.Begin(null, false);

            var targetElements = XmlElementSerializer.GetElements(context.XElement, false, _rootTarget, -1)?.ToArray();

            if (targetElements == null)
                return;

            foreach (var targetElement in targetElements)
                _listBuilder?.Add(_serializer.Deserialize(targetElement));

            SetValue(target, _listBuilder?.End());
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            _listBuilder?.Begin(GetValue(target), true);

            var content = _listBuilder?.Objects;
            var targetElements = XmlElementSerializer.GetElements(context.XElement, true, _rootTarget, content?.Length ?? 0)?.ToArray();

            if (targetElements == null)
                throw new InvalidOperationException("Serialize No Data Returned");

            if (content == null) return;

            for (var i = 0; i < content.Length; i++)
                _serializer.Serialize(content[i], targetElements[i]);

            _listBuilder?.End();
        }

        public override Exception? VerifyError()
        {
            var e = base.VerifyError() ?? _serializer.VerifException() ?? _listBuilder?.VerifyError();

            if (_rootTarget == null)
                e = new ArgumentNullException(nameof(_rootTarget), @"Path to Elements: null");

            return e;
        }
    }
}