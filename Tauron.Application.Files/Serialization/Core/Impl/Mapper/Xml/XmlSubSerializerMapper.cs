using System;
using System.Xml.Linq;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlSubSerializerMapper : GenericSubSerializerMapper<XmlElementContext>
    {
        private readonly XmlElementTarget? _target;
        private bool _useSnapShot;

        public XmlSubSerializerMapper(string? membername, Type targetType,
            ISubSerializer? serializer, XmlElementTarget? target)
            : base(membername, targetType, serializer)
        {
            _target = target;
        }

        protected override SerializationContext GetRealContext(XmlElementContext origial, SerializerMode mode)
        {
            if (!(XmlElementSerializer.GetElement(origial.XElement, mode == SerializerMode.Serialize, _target) is XElement ele))
            {
                _useSnapShot = false;
                return origial.Original;
            }

            _useSnapShot = true;
            return origial.Original.CreateSnapshot(ele.Value);
        }

        protected override void PostProgressing(SerializationContext context)
        {
            if (_useSnapShot)
                context.Dispose();
        }

        public override Exception? VerifyError()
        {
            var e = base.VerifyError();

            if (_target?.TargetType == XmlElementTargetType.Attribute)
                e = new SerializerElementException("The Subserializer does not Support Attributes");

            return e;
        }
    }
}