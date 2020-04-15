﻿using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper.Xml
{
    internal class XmlElementTarget
    {
        [CanBeNull]
        public string Name { get; set; }

        [CanBeNull]
        public XNamespace XNamespace { get; set; }

        public XmlElementTargetType TargetType { get; set; }

        [CanBeNull]
        public XmlElementTarget SubElement { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            string postfix;

            switch (TargetType)
            {
                case XmlElementTargetType.Root:
                    postfix = "-Root";
                    break;
                case XmlElementTargetType.Attribute:
                    postfix = "-Attribute";
                    break;
                case XmlElementTargetType.Element:
                    postfix = "-Element";
                    break;
                default:
                    postfix = "-Unkown";
                    break;
            }

            builder.AppendFormat("{0}={1}", (Name ?? "NoName") + postfix,
                                 SubElement == null ? "End" : SubElement.ToString());

            return builder.ToString();
        }
    }
}