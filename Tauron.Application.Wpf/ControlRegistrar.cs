﻿using System;
using System.Windows.Threading;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Wpf.UI;

namespace Tauron.Application.Wpf
{
    public sealed class ControlRegistrar : RegistrationStrategy
    {
        public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
        {
            var modelType = descriptor.ImplementationType.GetCustomAttribute<ControlAttribute>()?.ModelType;
            if (modelType != null)
                AutoViewLocation.AddPair(descriptor.ImplementationType, modelType);

            var trampoline = new Trampoline(descriptor.ImplementationType);

            services.Add(ServiceDescriptor.Transient(descriptor.ImplementationType, serviceProvider => DispatcherInvoke(serviceProvider, trampoline.Create)));
        }

        private static object DispatcherInvoke(IServiceProvider dipatcher, Func<IServiceProvider, object> invoker)
        {
            var dis = System.Windows.Application.Current.Dispatcher;
            return dis.Invoke(() => invoker(dipatcher));
        }

        private class Trampoline
        {
            private readonly Type _targetType;
            private ObjectFactory? _fac;

            public Trampoline(Type targetType) => _targetType = targetType;

            public object Create(IServiceProvider provider)
            {
                if (_fac != null) return _fac.Invoke(provider, Array.Empty<object>());

                _fac = ActivatorUtilities.CreateFactory(_targetType, Type.EmptyTypes);
                return _fac(provider, Array.Empty<object>());
            }
        }
    }
}