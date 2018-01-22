// The file DefaultExtension.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultExtension.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default extension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The default extension.</summary>
    public sealed class DefaultExtension : IContainerExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public void Initialize(ComponentRegistry components)
        {
            components.Register<IMetadataFactory, MetadataFactory>();
            components.Register<IImportSelectorChain, ImportSelectorChain>();
            components.Register<IExportFactory, DefaultExportFactory>(DefaultExportFactory.Factory);
            components.Register<ICache, BuildCache>();
            components.Register<IEventManager, EventManager>();
            components.Register<IProxyService, ProxyService>();

            components.Register<IStrategy, CacheStrategy>();
            components.Register<IStrategy, LiftimeStrategy>();
            components.Register<IStrategy, InstantiationStrategy>();
            components.Register<IStrategy, InterceptionStrategy>();
            components.Register<IStrategy, InjectionStrategy>();
            components.Register<IStrategy, NodifyBuildCompledStrategy>();

            var chain = components.Get<IImportSelectorChain>();
            chain.Register(new FieldImportSelector());
            chain.Register(new PropertyImportSelector());
            chain.Register(new MethodImportSelector());
            chain.Register(new EventImportSelector());
        }

        #endregion
    }
}