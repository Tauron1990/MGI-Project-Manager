// The file InterceptionStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="InterceptionStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The interception strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The interception strategy.</summary>
    public class InterceptionStrategy : StrategyBase
    {
        #region Static Fields

        /// <summary>The _generator.</summary>
        private static ProxyGenerator _generator;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public override void Initialize(ComponentRegistry components)
        {
            _generator = components.Get<IProxyService>().GenericGenerator;
        }

        /// <summary>
        ///     The on build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnBuild(IBuildContext context)
        {
            if (!context.CanUseBuildUp()) return;

            context.ErrorTracer.Phase = "Setting up ObjectContext for " + context.Metadata;

            var contextPolicy = context.Policys.Get<ObjectContextPolicy>();

            ObjectContext objectContext;

            var name = contextPolicy.ContextName;
            if (name != null)
            {
                objectContext = ContextManager.GetContext(name, context.Target);
            }
            else
            {
                var holder = context.Target as IContextHolder;
                name = Guid.NewGuid().ToString();

                if (holder != null)
                {
                    if (holder.Context == null) holder.Context = ContextManager.GetContext(name, context.Target);

                    objectContext = holder.Context;
                }
                else
                {
                    var obj = context.Target;

                    objectContext = ContextManager.GetContext(name, obj);
                }
            }

            foreach (var contextProperty in contextPolicy.ContextPropertys) contextProperty.Item1.Register(objectContext, contextProperty.Item2, context.Target);

            var policy = context.Policys.Get<InterceptionPolicy>();

            if (policy == null || context.Target == null) return;

            var options = new ProxyGenerationOptions {Selector = new InternalInterceptorSelector()};

            try
            {
                if (ProxyUtil.IsProxy(context.Target)) return;

                context.ErrorTracer.Phase = "Creating Proxy with Target for " + context.Metadata;

                lock (_generator)
                {
                    context.Target = _generator.CreateClassProxyWithTarget(
                        context.ExportType,
                        context.Target,
                        options,
                        policy.MemberInterceptor.Select(
                                mem =>
                                    mem.Value)
                            .ToArray());
                }
            }
            finally
            {
                foreach (var result in
                    policy.MemberInterceptor.Select(mem => mem.Key).Where(attr => attr != null))
                    result.Initialize(context.Target, objectContext, name);
            }
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPerpare(IBuildContext context)
        {
            if (!context.CanUseBuildUp()) return;

            context.ErrorTracer.Phase = "Reciving Interception Informations for " + context.Metadata;

            var contextPolicy = new ObjectContextPolicy
            {
                ContextName =
                    context.Metadata.Metadata.TryGetAndCast<string>(
                        AopConstants.ContextMetadataName)
            };

            foreach (var memberInfo in context.ExportType.GetMembers(AopConstants.DefaultBindingFlags))
            {
                var attrs =
                    memberInfo.GetAllCustomAttributes<ObjectContextPropertyAttribute>();
                foreach (var objectContextPropertyAttribute in attrs) contextPolicy.ContextPropertys.Add(Tuple.Create(objectContextPropertyAttribute, memberInfo));
            }

            context.Policys.Add(contextPolicy);

            var attr = context.Metadata.Metadata.TryGetAndCast<InterceptAttribute>(AopConstants.InterceptMetadataName);

            object meta;
            if (context.Metadata.Metadata.TryGetValue("IgnoreIntercepion", out meta))
                try
                {
                    if ((bool) meta)
                        return;
                }
                catch (InvalidCastException)
                {
                }

            if (attr == null) return;

            var policy = new InterceptionPolicy {InterceptAttribute = attr};
            var temp = attr.Create();
            if (temp != null) policy.MemberInterceptor.Add(new KeyValuePair<MemberInterceptionAttribute, IInterceptor>(null, temp));

            foreach (var attribute in
                context.ExportType.GetAllCustomAttributes<MemberInterceptionAttribute>())
                policy.MemberInterceptor.Add(
                    new KeyValuePair<MemberInterceptionAttribute, IInterceptor>(
                        attribute,
                        attribute.Create(context.ExportType)));

            foreach (var member in context.ExportType.GetMembers(AopConstants.DefaultBindingFlags))
            {
                var intattrs = member.GetAllCustomAttributes<MemberInterceptionAttribute>();
                foreach (var interceptionAttribute in intattrs)
                {
                    var temp2 = interceptionAttribute.Create(member);
                    policy.MemberInterceptor.Add(
                        new KeyValuePair<MemberInterceptionAttribute, IInterceptor>(
                            interceptionAttribute, temp2));
                }
            }

            context.Policys.Add(policy);
        }

        #endregion
    }
}