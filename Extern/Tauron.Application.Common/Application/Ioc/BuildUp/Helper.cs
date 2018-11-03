using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public static class Helper
    {

        public static IEnumerable<(Type Type, string Name, bool Optional)> MapParameters([NotNull] MethodBase info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            foreach (var parameterInfo in info.GetParameters())
            {
                var attr = parameterInfo.GetCustomAttribute<InjectAttribute>();
                if (attr == null) yield return (parameterInfo.ParameterType, null, true);
                else
                    yield return
                        (attr.Interface ?? parameterInfo.ParameterType, attr.ContractName, attr.Optional);
            }
        }

        [NotNull]
        public static Func<IBuildContext, ProxyGenerator, object> WriteDefaultCreation([NotNull] IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var type = context.Metadata.Export.ImplementType;

            var construcors = type.GetConstructors(AopConstants.DefaultBindingFlags);
            ConstructorInfo constructor = null;
            foreach (var constructorInfo in
                construcors.Where(constructorInfo => constructorInfo.GetCustomAttribute<InjectAttribute>() != null))
            {
                if (constructor != null) throw new InvalidOperationException("Too manay Constructors");

                constructor = constructorInfo;
            }

            if (constructor == null) constructor = construcors.First(con => con.GetParameters().Length == 0);

            context.ErrorTracer.Phase = "Returning Default Creation for " + context.Metadata;

            return (build, service) =>
            {
                if (build == null)
                    throw new ArgumentNullException(nameof(build));
                //CContract.Requires<ArgumentNullException>(build != null, "build");
                //CContract.Requires<ArgumentNullException>(service != null, "service");
                //CContract.Ensures(CContract.Result<object>() != null);

                var parameters = from parm in MapParameters(constructor)
                    select TryResolveConstructorParameter(parm, build);


                var policy = build.Policys.Get<InterceptionPolicy>();

                if (policy == null) return constructor.Invoke(parameters.ToArray());

                build.ErrorTracer.Phase = "Creating Direct Proxy for " + build.Metadata;

                return service.CreateClassProxy(
                    build.ExportType,
                    null,
                    new ProxyGenerationOptions
                    {
                        Selector =
                            new InternalInterceptorSelector
                                ()
                    },
                    parameters.ToArray(),
                    policy.MemberInterceptor.Select(mem => mem.Value)
                        .ToArray());
            };
        }


        private static object TryResolveConstructorParameter((Type Type, string Name, bool Optional) parm, IBuildContext context)
        {
            var errorTrancer = new ErrorTracer();
            var tempExport = context.Container.FindExport(parm.Type, parm.Name, errorTrancer, true);
            if (tempExport != null) return context.Container.BuildUp(tempExport, errorTrancer, context.Parameters);
            switch (context.Parameters)
            {
                case null when parm.Optional:
                    return null;
                case null when !parm.Optional:
                    throw new BuildUpException(errorTrancer);
                default:
                    var tempRegistry = new ExportRegistry();

                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var parameter in context.Parameters) tempRegistry.Register(parameter.CreateExport() ?? throw new InvalidOperationException(), 0);

                    var data = tempRegistry.FindOptional(parm.Type, parm.Name, context.ErrorTracer);
                    return data == null ? null : context.Container.BuildUp(data, context.ErrorTracer, context.Parameters);
            }
        }
    }
}