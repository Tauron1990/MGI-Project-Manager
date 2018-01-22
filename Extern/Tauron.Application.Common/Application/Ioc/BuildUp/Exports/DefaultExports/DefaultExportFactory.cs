// The file DefaultExportFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultExportFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default export factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The default export factory.</summary>
    public sealed class DefaultExportFactory : IExportFactory
    {
        #region Fields

        /// <summary>The _chain.</summary>
        private IImportSelectorChain _chain;

        #endregion

        #region Public Properties

        public static readonly DefaultExportFactory Factory = new DefaultExportFactory();

        /// <summary>Gets the technology name.</summary>
        /// <value>The technology name.</value>
        public string TechnologyName => AopConstants.DefaultExportFactoryName;

        #endregion

        #region Public Methods and Operators

        private DefaultExportFactory()
        {
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public void Initialize(ComponentRegistry components)
        {
            _chain = components.Get<IImportSelectorChain>();
        }

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="IExport" />.
        /// </returns>
        public IExport Create([NotNull] Type type, ref int level)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!DefaultExport.IsExport(type)) return null;

            var export = new DefaultExport(
                type,
                new ExternalExportInfo(false, false, true, true, null, string.Empty),
                false);

            var attr = type.GetCustomAttribute<ExportLevelAttribute>();
            if (attr != null) level = attr.Level;


            export.ImportMetadata = _chain.SelectImport(export);

            return export;
        }

        /// <summary>
        ///     The create anonymos.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="IExport" />.
        /// </returns>
        public IExport CreateAnonymos([NotNull] Type type, object[] args)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var export = new DefaultExport(
                type,
                new ExternalExportInfo(
                    true,
                    true,
                    true,
                    true,
                    (con, ps) => Activator.CreateInstance(type, args),
                    type.Name),
                true);

            export.ImportMetadata = _chain.SelectImport(export);
            return export;
        }

        /// <summary>
        ///     The create anonymos with target.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="IExport" />.
        /// </returns>
        [NotNull]
        public IExport CreateAnonymosWithTarget([NotNull] Type type, [NotNull] object target)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (target == null) throw new ArgumentNullException(nameof(target));
            var info = new ExternalExportInfo(true, true, true, true, (context, service) => target, null);

            var export = new DefaultExport(type, info, true);
            export.ImportMetadata = _chain.SelectImport(export);

            return export;
        }

        /// <summary>
        ///     The create method export.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IExport" />.
        /// </returns>
        public IExport CreateMethodExport([NotNull] MethodInfo info, ref int currentLevel)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (!info.IsStatic || !DefaultExport.IsExport(info)) return null;

            var attr = info.GetCustomAttribute<ExportLevelAttribute>();
            if (attr != null) currentLevel = attr.Level;

            return new DefaultExport(
                info,
                new ExternalExportInfo(true, false, true, false, (arg1, arg2) => info.Invoke(null, null), info.Name),
                false);
        }

        #endregion
    }
}