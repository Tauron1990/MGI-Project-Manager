using System;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class CommandTargetAttribute : MemberInfoAttribute
    {
        public CommandTargetAttribute(string memberName)
            : base(memberName)
        {
        }

        public CommandTargetAttribute()
            : base(null)
        {
        }

        public string? CanExecuteMember { get; set; }
    }
}