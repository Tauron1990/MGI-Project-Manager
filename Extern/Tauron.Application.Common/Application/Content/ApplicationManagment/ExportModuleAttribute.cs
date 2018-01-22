using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [BaseTypeRequired(typeof(IModule))]
    public class ExportModuleAttribute : ExportAttribute
    {
        public ExportModuleAttribute() : base(typeof(IModule))
        {
        }
    }
}