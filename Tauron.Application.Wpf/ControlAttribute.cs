using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf
{
    [MeansImplicitUse, AttributeUsage(AttributeTargets.Class)]
    public class ControlAttribute : Attribute
    {
        public Type ModelType { get; }

        public ControlAttribute(Type modelType) => ModelType = modelType;
    }
}