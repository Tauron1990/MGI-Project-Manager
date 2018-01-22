// The file AggregareExportProvider.cs is part of Tauron.Application.Common.
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
// <copyright file="AggregareExportProvider.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The aggregare export provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The aggregare export provider.</summary>
    public class AggregareExportProvider : ExportProvider
    {
        #region Fields

        private readonly List<ExportResolver.AssemblyExportProvider> assemblys =
            new List<ExportResolver.AssemblyExportProvider>();

        private IExportFactory factory;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether broadcast changes.</summary>
        public override bool BroadcastChanges => true;

        /// <summary>Gets the technology.</summary>
        public override string Technology => AopConstants.DefaultExportFactoryName;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        public void Add(Assembly assembly)
        {
            var export = new ExportResolver.AssemblyExportProvider(assembly);
            assemblys.Add(export);

            if (factory != null)
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        export.CreateExports(factory).SelectMany(ex => ex.Item1.ExportMetadata),
                        Enumerable.Empty<ExportMetadata>()));
        }

        /// <summary>
        ///     The add rage.
        /// </summary>
        /// <param name="addAssemblys">
        ///     The add assemblys.
        /// </param>
        public void AddRange(IEnumerable<Assembly> addAssemblys)
        {
            if (addAssemblys.Count() == 0) return;

            var temp =
                addAssemblys.Select(asm => new ExportResolver.AssemblyExportProvider(asm)).ToArray();
            assemblys.AddRange(temp);

            if (factory != null)
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        temp.SelectMany(prov => prov.CreateExports(factory))
                            .SelectMany(ex => ex.Item1.ExportMetadata),
                        Enumerable.Empty<ExportMetadata>()));
        }

        /// <summary>
        ///     The create exports.
        /// </summary>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
        {
            this.factory = factory;
            return assemblys.SelectMany(prov => prov.CreateExports(factory));
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        public void Remove(Assembly assembly)
        {
            var export = new ExportResolver.AssemblyExportProvider(assembly);

            var index = assemblys.IndexOf(export);
            if (index != -1)
            {
                export = assemblys[index];
                if (!assemblys.Remove(export)) export = null;
            }
            else
            {
                export = null;
            }

            if (factory != null && export != null)
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        Enumerable.Empty<ExportMetadata>(),
                        export.CreateExports(factory).SelectMany(ex => ex.Item1.ExportMetadata)));
        }

        /// <summary>
        ///     The remove rage.
        /// </summary>
        /// <param name="removeAssemblys">
        ///     The remove assemblys.
        /// </param>
        public void RemoveRange(IEnumerable<Assembly> removeAssemblys)
        {
            IEnumerable<Assembly> assemblies = removeAssemblys as Assembly[] ?? removeAssemblys.ToArray();
            if (!assemblies.Any()) return;

            var temp =
                assemblies.Select(asm => new ExportResolver.AssemblyExportProvider(asm)).ToList();
            var indiexes =
                temp.Select(provider => assemblys.IndexOf(provider)).Where(index => index != -1).ToList();
            temp.Clear();

            foreach (var index in indiexes)
            {
                temp.Add(assemblys[index]);
                assemblys.RemoveAt(index);
            }

            if (factory != null)
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        Enumerable.Empty<ExportMetadata>(),
                        temp.SelectMany(prov => prov.CreateExports(factory))
                            .SelectMany(ex => ex.Item1.ExportMetadata)));
        }

        #endregion
    }
}