// The file MethodImportSelector.cs is part of Tauron.Application.Common.
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
// <copyright file="MethodImportSelector.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The method import selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The method import selector.</summary>
    public class MethodImportSelector : IImportSelector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The select.
        /// </summary>
        /// <param name="exportType">
        ///     The export.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" />.
        /// </returns>
        public IEnumerable<ImportMetadata> SelectImport(IExport exportType)
        {
            foreach (var methodInfo in exportType.ImplementType.GetMethods(AopConstants.DefaultBindingFlags))
            {
                var attr = methodInfo.GetCustomAttribute<InjectAttribute>();
                if (attr == null) continue;

                var meta = attr.CreateMetadata();

                if (methodInfo.GetParameters().Length == 1)
                {
                    yield return
                        new ImportMetadata(
                                           attr.Interface,
                                           attr.ContractName,
                                           exportType,
                                           methodInfo.Name,
                                           attr.Optional,
                                           meta);
                }
                else
                {
                    meta.Add(AopConstants.ParameterMetadataName, Helper.MapParameters(methodInfo).ToArray());

                    yield return
                        new ImportMetadata(
                                           null,
                                           null,
                                           exportType,
                                           methodInfo.Name,
                                           false,
                                           meta);
                }
            }
        }

        #endregion
    }
}