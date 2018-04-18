using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CompositeInterceptor : IImportInterceptor
    {
        private readonly List<IImportInterceptor> _interceptors;

        public CompositeInterceptor([NotNull] [ItemNotNull] List<IImportInterceptor> interceptors)
        {
            _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
        }

        public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
        {
            var returnValue = true;

            foreach (var importInterceptor in _interceptors.Where(importInterceptor => returnValue))
            {
                returnValue = importInterceptor.Intercept(member, metadata, target, ref value);
            }

            return returnValue;
        }
    }
}