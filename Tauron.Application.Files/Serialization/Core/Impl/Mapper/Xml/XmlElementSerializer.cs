using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlElementSerializer
    {
        private readonly SimpleConverter<string>? _converter;
        private readonly XmlElementTarget? _target;

        public XmlElementSerializer(XmlElementTarget? target, SimpleConverter<string>? converter)
        {
            _target = target;
            _converter = converter;
        }

        public static IEnumerable<XElement>? GetElements([NotNull] XElement ele, bool toWrite, XmlElementTarget? target, int count)
        {
            var currentElement = Argument.NotNull(ele, nameof(ele));
            if (target == null) return new[] {currentElement};

            while (true)
            {
                if (target.XNamespace == null) target.XNamespace = XNamespace.None;

                if (target.TargetType == XmlElementTargetType.Attribute)
                    throw new InvalidOperationException("Attributes Not Supported");

                var currentName = target.XNamespace + target.Name;

                if (target.SubElement != null)
                {
                    var temp = currentElement.Element(currentName);

                    if (temp == null)
                    {
                        if (!toWrite) return null;
                        temp = new XElement(currentName);
                        currentElement.Add(temp);
                    }

                    currentElement = temp;
                    target = target.SubElement;
                    continue;
                }

                IEnumerable<XElement> els = currentElement.Elements(currentName).ToArray();

                if (!toWrite) return els;

                var realCount = els.Count();

                if (realCount == count) return els;

                var elements = new List<XElement>(els);

                for (var i = realCount; i < count; i++)
                {
                    var temp = new XElement(currentName);
                    currentElement.Add(temp);
                    elements.Add(temp);
                }

                return elements;
            }
        }

        public static XObject? GetElement([NotNull] XElement ele, bool toWrite, XmlElementTarget? target)
        {
            var currentElement = ele;

            if (target == null || target.TargetType == XmlElementTargetType.Root && target.SubElement == null) return currentElement;

            while (true)
            {
                if (target.XNamespace == null) target.XNamespace = XNamespace.None;

                var currentName = target.XNamespace + target.Name;
                switch (target.TargetType)
                {
                    case XmlElementTargetType.Attribute:
                        var attr = currentElement.Attribute(currentName);
                        if (attr != null) return attr;
                        if (!toWrite) return null;

                        attr = new XAttribute(currentName, string.Empty);
                        currentElement.Add(attr);

                        return attr;
                    case XmlElementTargetType.Element:
                        var temp = currentElement.Element(currentName);
                        if (temp == null)
                        {
                            if (!toWrite) return null;
                            temp = new XElement(currentName);
                            currentElement.Add(temp);
                        }

                        currentElement = temp;

                        if (target.SubElement == null) return currentElement;

                        target = target.SubElement;
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public object? Deserialize([NotNull] XElement file)
        {
            Argument.NotNull(file, nameof(file));

            object? obj = null;
            ProgressElement(SerializerMode.Deserialize, GetElement(file, false, _target), ref obj);
            return obj;
        }

        public void Serialize([NotNull] object target, [NotNull] XElement file)
        {
            Argument.NotNull(target, nameof(target));
            Argument.NotNull(file, nameof(file));

            var obj = target;
            ProgressElement(SerializerMode.Serialize, GetElement(file, true, _target), ref obj);
        }

        private void ProgressString(SerializerMode mode, ref string str, ref object? obj)
        {
            switch (mode)
            {
                case SerializerMode.Deserialize:
                    obj = _converter?.ConvertBack(str);
                    break;
                case SerializerMode.Serialize:
                    str = _converter?.Convert(obj) ?? string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private void ProgressElement(SerializerMode mode, XObject? xobj, ref object? obj)
        {
            if (xobj == null) return;

            var attr = xobj as XAttribute;

            var str = attr != null ? attr.Value : ((XElement) xobj).Value;

            switch (mode)
            {
                case SerializerMode.Deserialize:
                    ProgressString(mode, ref str, ref obj);
                    break;
                case SerializerMode.Serialize:
                    ProgressString(mode, ref str, ref obj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            if (mode != SerializerMode.Serialize) return;

            if (attr != null) attr.Value = str;
            else ((XElement) xobj).Value = str;
        }

        public Exception? VerifException()
        {
            if (_converter == null) return new ArgumentNullException(nameof(_converter), @"Converter");

            var e = _converter.VerifyError();
            if (e != null) return e;

            return _target == null ? new ArgumentNullException(nameof(_target), @"Xml Tree") : null;
        }
    }
}