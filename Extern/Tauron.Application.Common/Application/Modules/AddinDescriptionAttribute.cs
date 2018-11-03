using System;

namespace Tauron.Application.Modules
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class AddinDescriptionAttribute : Attribute
    {
    }
}