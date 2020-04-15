using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlListSubSerializerMapper : MappingEntryBase<XmlElementContext>
    {
        private readonly ListBuilder _listBuilder;
        private readonly XmlElementTarget? _rootTarget;
        private readonly ISubSerializer? _serializer;
        private readonly XmlElementTarget? _target;

        public XmlListSubSerializerMapper(string? membername, Type targetType, XmlElementTarget? target, XmlElementTarget? rootTarget,
            ISubSerializer? serializer)
            : base(membername, targetType)
        {
            _target = target;
            _rootTarget = rootTarget;
            _serializer = serializer;
            _listBuilder = new ListBuilder(MemberType);
        }

        protected override void Deserialize(object target, XmlElementContext context)
        {
            _listBuilder.Begin(null, false);

            foreach (var xElement in GetElements(false, context.XElement, -1) ?? Enumerable.Empty<XElement>())
                _listBuilder.Add(_serializer?.Deserialize(context.Original.CreateSnapshot(xElement.ToString())));

            SetValue(target, _listBuilder.End());
        }

        protected override void Serialize(object target, XmlElementContext context)
        {
            _listBuilder.Begin(GetValue(target), true);

            var obj = _listBuilder.Objects;
            var eles = GetElements(true, context.XElement, obj.Length)?.ToArray();

            for (var i = 0; i < obj.Length; i++)
            {
                var snapshot = context.Original.CreateSnapshot(new byte[0]);

                _serializer?.Serialize(snapshot, obj[i]);

                var targetEle = eles?[i];
                var stringVal = snapshot.TextReader.ReadToEnd();

                if (stringVal.StartsWith("<") && stringVal.EndsWith(">")) targetEle?.Add(XElement.Parse(stringVal));
                else if (targetEle != null) targetEle.Value = stringVal;
            }

            _listBuilder.End();
        }

        private IEnumerable<XElement>? GetElements(bool forWrite, [NotNull] XElement context, int count)
        {
            var realRoot = XmlElementSerializer.GetElement(context, forWrite, _rootTarget) as XElement;

            if (realRoot == null && forWrite) throw new InvalidOperationException("Attributes not Supported");

            return realRoot == null
                ? Enumerable.Empty<XElement>()
                : XmlElementSerializer.GetElements(realRoot, forWrite, _target, count);
        }

        public override Exception? VerifyError()
        {
            var e = base.VerifyError() ?? _listBuilder.VerifyError();

            if (_rootTarget == null)
                e = new ArgumentNullException(nameof(_rootTarget), @"Path to Elements: null");

            if (_serializer == null)
                e = new ArgumentNullException(nameof(_serializer), @"SubSerializer is Null");

            return e;
        }
    }
}