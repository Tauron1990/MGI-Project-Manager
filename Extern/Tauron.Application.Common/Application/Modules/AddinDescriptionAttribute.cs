using System;
using JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class AddinDescriptionAttribute : Attribute
    {
    }
}