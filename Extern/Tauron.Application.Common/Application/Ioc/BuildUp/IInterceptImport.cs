using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public interface IImportInterceptor
    {
        bool Intercept([NotNull] MemberInfo member, [NotNull] ImportMetadata metadata, [NotNull] object target,
            [CanBeNull] ref object value);
    }
}