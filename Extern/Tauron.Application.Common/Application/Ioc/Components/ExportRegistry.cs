// The file ExportRegistry.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportRegistry.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export registry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The export registry.</summary>
    [PublicAPI]
    public sealed class ExportRegistry
    {
        private class ExportEntry : GroupDictionary<Type, IExport>
        {
            public ExportEntry()
                : base(true)
            {
            }
        }

        private class ExportList : SortedList<int, ExportEntry>
        {
            private class DescendingComparer : IComparer<int>
            {
                public int Compare(int x, int y)
                {
                    if (x < y)
                        return 1;
                    if (x > y)
                        return -1;
                    return 0;
                }
            }

            public ExportList()
                : base(new DescendingComparer())
            {
            }

            public void Add(int location, [NotNull] Type type, [NotNull] IExport export)
            {
                if (TryGetValue(location, out var entry))
                {
                    entry[type].Add(export);
                }
                else
                {
                    entry = new ExportEntry();
                    entry[type].Add(export);
                    Add(location, entry);
                }
            }

            [CanBeNull]
            public IEnumerable<ExportMetadata> Lookup([NotNull] Type type, [CanBeNull] string contractName, int at)
            {
                var realExports = new HashSet<ExportMetadata>();

                foreach (var pair in this.Where(p => p.Key <= at))
                {
                    if (pair.Value.TryGetValue(type, out var exports))
                        exports.SelectMany(ep => ep.SelectContractName(contractName)).Foreach(ex => realExports.Add(ex));
                }

                return realExports.Count == 0 ? null : realExports;
            }

            public void RemoveValue([NotNull] IExport export)
            {
                foreach (var value in Values.ToArray()) value.RemoveValue(export);
            }
        }

        #region Fields

        /// <summary>The _registrations.</summary>
        private readonly ExportList _registrations = new ExportList();

        #endregion

        #region Public Methods and Operators

        [NotNull]
        public IEnumerable<ExportMetadata> FindAll([NotNull] Type type, [CanBeNull] string contractName, [NotNull] ErrorTracer errorTracer, int limit = int.MaxValue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));

            try
            {
                lock (this)
                {
                    errorTracer.Phase = "Getting Exports by Type (" + type + ")";
                    var regs = _registrations.Lookup(type, contractName, limit);

                    if (regs == null)
                        return Enumerable.Empty<ExportMetadata>();

                    errorTracer.Phase = "Filtering Exports by Contract Name (" + contractName + ")";
                    return regs.Where(exp => exp != null);
                }
            }
            catch (Exception e)
            {
                errorTracer.Exception   = e;
                errorTracer.Exceptional = true;
                errorTracer.Export      = ErrorTracer.FormatExport(type, contractName);

                return Enumerable.Empty<ExportMetadata>();
            }
        }

        [CanBeNull]
        public ExportMetadata FindOptional([NotNull] Type type, [CanBeNull] string contractName, [NotNull] ErrorTracer errorTracer, int level = int.MaxValue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            lock (this)
            {
                var arr = FindAll(type, contractName, errorTracer).ToArray();

                if (errorTracer.Exceptional) return null;

                errorTracer.Phase = "Getting Single Instance";

                if (arr.Length <= 1) return arr.Length == 0 ? null : arr[0];

                errorTracer.Exceptional = true;
                errorTracer.Exception   = new InvalidOperationException("More then One Export Found");
                errorTracer.Export      = ErrorTracer.FormatExport(type, contractName);

                return arr.Length == 0 ? null : arr[0];
            }
        }


        [CanBeNull]
        public ExportMetadata FindSingle([NotNull] Type type, [NotNull] string contractName, [NotNull] ErrorTracer errorTracer, int level = int.MaxValue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            var temp = FindOptional(type, contractName, errorTracer, level);
            if (errorTracer.Exceptional) return null;
            if (temp != null) return temp;

            errorTracer.Exceptional = true;
            errorTracer.Exception   = new InvalidOperationException("No Export Found");
            errorTracer.Export      = ErrorTracer.FormatExport(type, contractName);

            return null;
        }

        public void Register([NotNull] IExport export, int level)
        {
            if (export == null) throw new ArgumentNullException(nameof(export));
            lock (this)
                foreach (var type in export.Exports)
                    _registrations.Add(level, type, export);
        }

        public void Remove([NotNull] IExport export)
        {
            if (export == null) throw new ArgumentNullException(nameof(export));
            lock (this) _registrations.RemoveValue(export);
        }

        #endregion
    }
}