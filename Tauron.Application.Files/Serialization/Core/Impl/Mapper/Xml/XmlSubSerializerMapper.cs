﻿using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal sealed class XmlSubSerializerMapper : GenericSubSerializerMapper<XmlElementContext>
    {
        private readonly XmlElementTarget _target;
        private          bool             _useSnapShot;

        public XmlSubSerializerMapper([CanBeNull] string         membername, [NotNull]   Type             targetType,
                                      [CanBeNull] ISubSerializer serializer, [CanBeNull] XmlElementTarget target)
            : base(membername, targetType, serializer) => _target = target;

        protected override SerializationContext GetRealContext(XmlElementContext origial, SerializerMode mode)
        {
            var ele = XmlElementSerializer.GetElement(origial.XElement, mode == SerializerMode.Serialize, _target) as XElement;
            if (ele == null)
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

        public override Exception VerifyError()
        {
            var e = base.VerifyError();

            if (_target.TargetType == XmlElementTargetType.Attribute)
                e = new SerializerElementException("The Subserializer does not Support Attributes");

            return e;
        }
    }
}