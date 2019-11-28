using System;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload.Core
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class CommandTargetAttribute : MemberInfoAttribute
    {
        public string? CanExecuteMember { get; set; }

        public CommandTargetAttribute(string memberName)
            : base(memberName)
        {
        }

        public CommandTargetAttribute()
            : base(null)
        {
        }
    }
}