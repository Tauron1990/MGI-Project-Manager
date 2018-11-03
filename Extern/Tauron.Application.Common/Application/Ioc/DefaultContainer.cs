// The file DefaultContainer.cs is part of Tauron.Application.Common.
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
// <copyright file="DefaultContainer.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The default container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc
{
    public sealed class DefaultContainer : MarshalByRefObject, IContainer
    {
        #region Constructors and Destructors

        public DefaultContainer()
        {
            _extensions = new List<IContainerExtension>();
            _componetnts = new ComponentRegistry();
            _exports = new ExportRegistry();
            _exportproviders = new ExportProviderRegistry();
            _exportproviders.ExportsChanged += ExportsChanged;
            Register(new DefaultExtension());
            _buildEngine = new BuildEngine(this, _exportproviders, _componetnts);
            _exports.Register(DefaultExportFactory.Factory.CreateAnonymosWithTarget(typeof(IContainer), this), 0);
        }

        #endregion

        #region Methods

        private void ExportsChanged(object sender, ExportChangedEventArgs e)
        {
            var temp = new List<IExport>();

            foreach (var exportMetadata in
                e.Removed.Where(exportMetadata => !temp.Contains(exportMetadata.Export)))
            {
                _exports.Remove(exportMetadata.Export);
                temp.Add(exportMetadata.Export);
            }

            temp.Clear();

            foreach (
                var exportMetadata in e.Added.Where(exportMetadata => !temp.Contains(exportMetadata.Export)))
            {
                var attr = exportMetadata.Export.ImplementType.GetCustomAttribute<ExportLevelAttribute>();
                if (attr == null)
                    attr = exportMetadata.Export.ImplementType.Assembly.GetCustomAttribute<ExportLevelAttribute>();

                _exports.Register(exportMetadata.Export, attr == null ? 0 : attr.Level);
                temp.Add(exportMetadata.Export);
            }
        }

        #endregion

        #region Fields

        /// <summary>The _build engine.</summary>
        private readonly BuildEngine _buildEngine;

        /// <summary>The _componetnts.</summary>
        private readonly ComponentRegistry _componetnts;

        /// <summary>The _exportproviders.</summary>
        private readonly ExportProviderRegistry _exportproviders;

        /// <summary>The _exports.</summary>
        private readonly ExportRegistry _exports;

        /// <summary>The _extensions.</summary>
        private readonly List<IContainerExtension> _extensions;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        public object BuildUp([NotNull] ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            try
            {
                errorTracer.Export = data.ToString();
                return _buildEngine.BuildUp(data.Export, data.ContractName, errorTracer, parameters);
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;

                throw new BuildUpException(errorTracer);
                //throw new BuildUpException(string.Format("Build Error: {0}", data), e);
            }
        }

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="toBuild">
        ///     The object.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp(object toBuild, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            try
            {
                return _buildEngine.BuildUp(toBuild, errorTracer, parameters);
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;

                throw new BuildUpException(errorTracer);
                //throw new BuildUpException(string.Format("Instance Build Error: {0}", toBuild), e);
            }
        }

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="constructorArguments">
        ///     The constructor arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp(Type type, ErrorTracer errorTracer, BuildParameter[] buildParameters, params object[] constructorArguments)
        {
            try
            {
                return _buildEngine.BuildUp(type, constructorArguments, errorTracer, buildParameters);
            }
            catch (Exception e)
            {
                if (e is BuildUpException) throw;

                errorTracer.Exception = e;
                throw new BuildUpException(errorTracer);
                //throw new BuildUpException(string.Format("Type Build Error: {0}", type), e);
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _componetnts.Dispose();
            _exportproviders.Dispose();
        }

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="isOptional">
        ///     The optional.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional)
        {
            try
            {
                return isOptional ? _exports.FindOptional(interfaceType, name, errorTracer) : FindExport(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return null;
        }

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            try
            {
                return _exports.FindSingle(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;

                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return null;
        }

        /// <summary>
        ///     The find exports.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            try
            {
                return _exports.FindAll(interfaceType, name, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;

                //if (e is FindExportException) throw;

                //throw new FindExportException(string.Format("Resolve Failed: [{0}|{1}]", interfaceType, name), e);
            }

            return null;
        }

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional, int level)
        {
            return isOptional
                ? _exports.FindOptional(interfaceType, name, errorTracer, level)
                : _exports.FindSingle(interfaceType, name, errorTracer, level);
        }

        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer, int level)
        {
            return _exports.FindAll(interfaceType, name, errorTracer, level);
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportType">
        ///     The export.
        /// </param>
        /// <param name="level"></param>
        public void Register(IExport exportType, int level)
        {
            _exports.Register(exportType, level);
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportResolver">
        ///     The export.
        /// </param>
        public void Register(ExportResolver exportResolver)
        {
            exportResolver.Fill(_componetnts, _exports, _exportproviders);
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="extension">
        ///     The extension.
        /// </param>
        public void Register(IContainerExtension extension)
        {
            extension.Initialize(_componetnts);
            _extensions.Add(extension);
        }

        #endregion
    }
}