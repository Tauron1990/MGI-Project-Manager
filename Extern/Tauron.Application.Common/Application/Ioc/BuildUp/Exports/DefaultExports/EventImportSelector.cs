// The file EventImportSelector.cs is part of Tauron.Application.Common.
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
// <copyright file="EventImportSelector.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event import selector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The event import selector.</summary>
    public class EventImportSelector : IImportSelector
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
            foreach (var eventInfo in exportType.ImplementType.GetEvents(AopConstants.DefaultBindingFlags))
            {
                var attr = eventInfo.GetCustomAttribute<InjectEventAttribute>();
                if (attr != null)
                    yield return
                        new ImportMetadata(
                            eventInfo.EventHandlerType,
                            attr.Topic,
                            exportType,
                            eventInfo.Name,
                            true,
                            null);
            }

            foreach (var methodInfo in exportType.ImplementType.GetMethods(AopConstants.DefaultBindingFlags))
            {
                var attr = methodInfo.GetCustomAttribute<InjectEventAttribute>();
                if (attr == null) continue;

                yield return
                    new ImportMetadata(
                        methodInfo.GetType(),
                        attr.Topic,
                        exportType,
                        methodInfo.Name,
                        true,
                        new Dictionary<string, object>
                        {
                            {AopConstants.EventMetodMetadataName, true},
                            {AopConstants.EventTopicMetadataName, attr.Topic}
                        });
            }
        }

        #endregion
    }
}