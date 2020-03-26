using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class ControlAttribute : Attribute
    {
        public ControlAttribute(Type modelType) => ModelType = modelType;

        public Type ModelType { get; }
    }
}