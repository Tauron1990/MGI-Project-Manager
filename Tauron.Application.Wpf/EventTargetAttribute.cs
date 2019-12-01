using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf 
{ 
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class EventTargetAttribute : MemberInfoAttribute
    {
        public Type? Converter { get; set; }

        public EventTargetAttribute(string? memberName)
            : base(memberName){}

        public EventTargetAttribute()
            : base(null){}

    }
}