// The file Helper.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The helper.</summary>
    [PublicAPI]
    public static class Helper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The map parameters.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<Tuple<Type, string, bool>> MapParameters([NotNull] MethodBase info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            foreach (var parameterInfo in info.GetParameters())
            {
                var attr = parameterInfo.GetCustomAttribute<InjectAttribute>();
                if (attr == null) yield return Tuple.Create<Type, string, bool>(parameterInfo.ParameterType, null, true);
                else
                    yield return
                        Tuple.Create(attr.Interface ?? parameterInfo.ParameterType, attr.ContractName, attr.Optional);
            }
        }

        /// <summary>
        ///     The write default creation.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="Func" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
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


        private static object TryResolveConstructorParameter(Tuple<Type, string, bool> parm, IBuildContext context)
        {
            var temp = context.Container.Resolve(parm.Item1, parm.Item2, parm.Item3, context.Parameters);
            if (temp != null) return temp;
            if (context.Parameters == null) return null;

            ExportRegistry tempRegistry = new ExportRegistry();

            foreach (var parameter in context.Parameters)
                tempRegistry.Register(parameter.CreateExport() ?? throw new InvalidOperationException(), 0);

            var data = tempRegistry.FindOptional(parm.Item1, parm.Item2, context.ErrorTracer);
            if (data == null) return null;

            return context.Container.BuildUp(data, context.ErrorTracer, context.Parameters);
        }

        #endregion
    }
}