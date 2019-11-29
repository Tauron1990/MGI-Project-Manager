using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
    public abstract class MemberInfoAttribute : Attribute
    {
        protected MemberInfoAttribute(string? memberName) => MemberName = memberName;

        public string? MemberName { get; }
        
        public bool Synchronize { get; set; }
        
        public static IEnumerable<Tuple<string, MemberInfo>> GetMembers<TAttribute>(Type targetType)
            where TAttribute : MemberInfoAttribute
        {
            return targetType.FindMemberAttributes<TAttribute>(true)
                    .Select(attribute => Tuple.Create(attribute.Item2.ProvideMemberName(attribute.Item1), attribute.Item1));
        }

        public static void InvokeMembers<TAttribute>(object instance, string targetMember, params object[] parameters)
            where TAttribute : MemberInfoAttribute
        {
            Argument.NotNull(instance, nameof(instance));
            Argument.NotNull(targetMember, nameof(targetMember));

            foreach (var member in
                GetMembers<TAttribute>(instance.GetType()).Where(member => member.Item1 == targetMember))
                member.Item2.SetInvokeMember(instance, Argument.NotNull(parameters, nameof(parameters)));
        }
        
        public virtual string ProvideMemberName(MemberInfo info) => MemberName ?? Argument.NotNull(info, nameof(info)).Name;
    }
}