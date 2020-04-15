using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlSerializerMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly XmlSerializer? _serializer;
        private readonly XmlElementTarget? _xmlElementTarget;

        public XmlSerializerMapper(string? membername, [NotNull] Type targetType,
            XmlSerializer? serializer, XmlElementTarget? xmlElementTarget)
            : base(membername, targetType)
        {
            _serializer = serializer;
            _xmlElementTarget = xmlElementTarget;
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            var obj = XmlElementSerializer.GetElement(context.XElement, false, _xmlElementTarget);
            if (!(obj is XElement ele))
                return;
            SetValue(target, _serializer?.Deserialize(ele.CreateReader(ReaderOptions.OmitDuplicateNamespaces)));
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            var obj = XmlElementSerializer.GetElement(context.XElement, true, _xmlElementTarget);
            if (!(obj is XElement))
                throw new InvalidOperationException("Attributes not Supported");
            _serializer?.Serialize(context.XElement.CreateWriter(), GetValue(target));
        }

        public override Exception? VerifyError()
        {
            var e = base.VerifyError();

            if (_serializer == null)
                e = new ArgumentNullException(nameof(_serializer), @"Serializer");
            if (_xmlElementTarget == null)
                e = new ArgumentNullException(nameof(_xmlElementTarget), @"Xml Tree");

            var target = _xmlElementTarget;

            while (target != null)
            {
                if (target.TargetType == XmlElementTargetType.Attribute)
                {
                    e = new SerializerElementException("Attributes Not Supported");
                    break;
                }

                target = target.SubElement;
            }

            return e;
        }
    }
}