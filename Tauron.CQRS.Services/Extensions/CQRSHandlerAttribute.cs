using System;
using JetBrains.Annotations;

namespace Tauron.CQRS.Services.Extensions
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class CQRSHandlerAttribute : Attribute
    {
        
    }
}