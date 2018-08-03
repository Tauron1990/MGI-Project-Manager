using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.Ioc;

namespace TestHelperLib
{
    [PublicAPI]
    public static class SetupApplication
    {
        private static List<Type> _typesToRegister = new List<Type>();
        private static List<Action> _setupActions = new List<Action>();

        public static IContainer Setup(bool addStandart, params Assembly[] asms)
        {
            foreach (var setupAction in _setupActions)
                setupAction();

            return CommonApplication.SetupTest(c =>
            {
                ExportResolver resolver = new ExportResolver();
                if (addStandart)
                {
                    resolver.AddAssembly(typeof(CommonApplication).Assembly);
                    resolver.AddAssembly(typeof(RepositoryFactory).Assembly);

                    foreach (var assembly in asms)
                        resolver.AddAssembly(assembly);
                }

                resolver.AddTypes(_typesToRegister);
                c.Register(resolver);
            });
        }

        public static void AddSetupAction(Action setupAction) => _setupActions.Add(setupAction);

        public static void AddTypes(params Type[] types) => _typesToRegister.AddRange(types);

        public static void Free()
        {
            _setupActions.Clear();
            _typesToRegister.Clear();
            CommonApplication.FreeTest();
        }
    }
}