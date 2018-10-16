// The file InjectionStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="InjectionStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The injection strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The injection strategy.</summary>
    public class InjectionStrategy : StrategyBase
    {
        #region Fields

        /// <summary>The _event manager.</summary>
        private IEventManager _eventManager;

        /// <summary>The _factory.</summary>
        private IMetadataFactory _factory;

        private IImportInterceptorFactory[] _interceptorFactories;

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
            _eventManager = components.Get<IEventManager>();
            _factory = components.Get<IMetadataFactory>();
            _interceptorFactories = components.GetAll<IImportInterceptorFactory>().ToArray();
        }

        /// <summary>
        ///     The on build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnBuild(IBuildContext context)
        {
            context.ErrorTracer.Phase = "Injecting Imports for " + context.Metadata;

            if (context.Target == null) return;

            foreach (var policy in context.Policys.GetAll<InjectMemberPolicy>())
            {
                policy.Injector.Inject(context.Target, context.Container, policy.Metadata,
                    policy.Interceptors == null ? null : new CompositeInterceptor(policy.Interceptors),
                    context.ErrorTracer, context.Parameters);

                if (context.ErrorTracer.Exceptional) return;
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

            context.ErrorTracer.Phase = "Loading Injections for " + context.Metadata;

            var members = context.ExportType.GetMembers(AopConstants.DefaultBindingFlags);

            List<IImportInterceptor> importInterceptors = null;
            var intpol = context.Policys.Get<ExternalImportInterceptorPolicy>();
            if (intpol != null) importInterceptors = intpol.Interceptors;

            foreach (
                var temp in
                _interceptorFactories.Select(
                        importInterceptorFactory => importInterceptorFactory.CreateInterceptor(context.Metadata))
                    .Where(temp => temp != null))
                if (importInterceptors == null)
                    importInterceptors = new List<IImportInterceptor> {temp};
                else
                    importInterceptors.Add(temp);

            foreach (var importMetadata in context.Metadata.Export.ImportMetadata)
            {
                var policy = new InjectMemberPolicy {Metadata = importMetadata, Interceptors = importInterceptors};

                var info = members.FirstOrDefault(inf => inf.Name == importMetadata.MemberName);
                if (info == null) continue;

                switch (info.MemberType)
                {
                    case MemberTypes.Event:
                        policy.Injector = new EventMemberInjector(importMetadata, _eventManager, info);
                        break;
                    case MemberTypes.Field:
                        policy.Injector = new FieldInjector(_factory, (FieldInfo) info, context.ResolverExtensions);
                        break;
                    case MemberTypes.Property:
                        policy.Injector =
                            new PropertyInjector(_factory, (PropertyInfo) info, context.ResolverExtensions);
                        break;
                    case MemberTypes.Method:
                        policy.Injector = new MethodInjector((MethodInfo) info, _factory, _eventManager,
                            context.ResolverExtensions);
                        break;
                }

                context.Policys.Add(policy);
            }
        }

        #endregion
    }
}