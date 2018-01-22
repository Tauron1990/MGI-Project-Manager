using System;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreExportAttribute : Attribute
    {
         
    }
}