using System;

namespace Tauron.CQRS.Common.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtoAttribute : Attribute
    {
        public string Name { get; }

        public Type Type { get; set; }

        public DtoAttribute(string name) => Name = name;
    }
}