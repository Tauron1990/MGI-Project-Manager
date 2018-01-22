// The file InstantiationStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="InstantiationStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The instantiation strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The instantiation strategy.</summary>
    public class InstantiationStrategy : StrategyBase
    {
        #region Fields

        /// <summary>The _service.</summary>
        private IProxyService _service;

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
            _service = components.Get<IProxyService>();
        }

        /// <summary>
        ///     The on create instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnCreateInstance(IBuildContext context)
        {
            var policy = context.Policys.Get<ConstructorPolicy>();
            if (policy == null) return;

            context.ErrorTracer.Phase = "Contruct Object for " + context.Metadata;

            context.Target = policy.Constructor.Invoke(context, policy.Generator); //(context, policy.Generator);
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPerpare(IBuildContext context)
        {
            if (context.Target != null) return;

            context.ErrorTracer.Phase = "Reciving Construtor Informations for " + context.Metadata;

            IImportInterceptor interceptor;

            context.Policys.Add(
                new ConstructorPolicy
                {
                    Constructor =
                        context.UseInternalInstantiation()
                            ? Helper.WriteDefaultCreation(context)
                            : context.Metadata.Export.ExternalInfo.Create,
                    Generator =
                        _service.Generate(context.Metadata, context.Metadata.Export.ImportMetadata.ToArray(),
                            out interceptor)
                });

            if (interceptor == null) return;

            var pol = context.Policys.Get<ExternalImportInterceptorPolicy>();

            if (pol != null)
            {
                pol.Interceptors.Add(interceptor);
            }
            else
            {
                pol = new ExternalImportInterceptorPolicy();
                pol.Interceptors.Add(interceptor);

                context.Policys.Add(pol);
            }
        }

        #endregion
    }
}