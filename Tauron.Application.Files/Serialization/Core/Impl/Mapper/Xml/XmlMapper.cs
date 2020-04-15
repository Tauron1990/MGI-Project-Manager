using System;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly XmlElementSerializer _serializer;

        public XmlMapper(string? membername, Type targetType, SimpleConverter<string>? converter, XmlElementTarget target)
            : base(membername, targetType)
        {
            if (converter == null && TargetMember != null)
                converter = ConverterFactory.CreateConverter(TargetMember, MemberType);

            _serializer = new XmlElementSerializer(target, converter);
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            SetValue(target, _serializer.Deserialize(context.XElement));
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            var realTarget = GetValue(target);
            if (realTarget == null) return;

            _serializer.Serialize(realTarget, context.XElement);
        }

        public override Exception? VerifyError()
        {
            return base.VerifyError() ?? _serializer.VerifException();
        }
    }
}