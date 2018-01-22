using System;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class ExportLevelAttribute : Attribute
    {
        public ExportLevelAttribute(int level)
        {
            Level = level;
        }

        public int Level { get; }
    }
}