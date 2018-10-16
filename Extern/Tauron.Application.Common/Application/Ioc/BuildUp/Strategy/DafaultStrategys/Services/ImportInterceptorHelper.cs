using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ImportInterceptorHelper
    {
        private readonly IImportInterceptor _interceptor;
        private readonly MemberInfo _member;
        private readonly ImportMetadata _metadata;
        private readonly object _target;

        public ImportInterceptorHelper([NotNull] IImportInterceptor interceptor, [NotNull] MemberInfo member,
            [NotNull] ImportMetadata metadata, [NotNull] object target)
        {
            _interceptor = interceptor;
            _member = member;
            _metadata = metadata;
            _target = target;
        }

        public bool Intercept([CanBeNull] ref object value)
        {
            return _interceptor.Intercept(_member, _metadata, _target, ref value);
        }
    }
}