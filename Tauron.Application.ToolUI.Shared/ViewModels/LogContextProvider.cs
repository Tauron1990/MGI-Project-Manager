using System;

namespace Tauron.Application.ToolUI.ViewModels
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LogContextProviderAttribute : Attribute
    {
        public Type ContextType { get; }

        public LogContextProviderAttribute(Type contextType) => ContextType = contextType;
    }
}