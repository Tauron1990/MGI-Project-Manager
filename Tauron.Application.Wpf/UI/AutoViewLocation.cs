using System;
using System.Collections.Generic;
using Catel.IoC;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Tauron.Application.Wpf.UI
{
    [ServiceDescriptor(typeof(AutoViewLocation), ServiceLifetime.Singleton)]
    [PublicAPI]
    public sealed class AutoViewLocation
    {
        private static readonly Dictionary<Type, Type> Views = new Dictionary<Type, Type>();

        private readonly ITypeFactory _provider;

        public AutoViewLocation(ITypeFactory provider)
        {
            _provider = provider;
        }

        public static AutoViewLocation Manager => DependencyResolverManager.Default.DefaultDependencyResolver.Resolve<AutoViewLocation>();

        public static void AddPair(Type view, Type model)
        {
            Views[model] = view;
        }

        public object? ResolveView(object viewModel)
        {
            return Views.TryGetValue(viewModel.GetType(), out var view) ? _provider.CreateInstanceWithParametersAndAutoCompletion(view, viewModel) : null;
        }
    }
}