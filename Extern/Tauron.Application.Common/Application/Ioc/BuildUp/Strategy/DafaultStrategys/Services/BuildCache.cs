// The file BuildCache.cs is part of Tauron.Application.Common.
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
// <copyright file="BuildCache.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The cache.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The cache.</summary>
    [PublicAPI]
    public sealed class BuildCache : ICache, IDisposable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildCache" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BuildCache" /> Klasse.
        ///     Initializes a new instance of the <see cref="BuildCache" /> class.
        /// </summary>
        public BuildCache()
        {
            WeakCleanUp.RegisterAction(OnCleanUp);
        }

        #endregion

        #region Fields

        /// <summary>The _global.</summary>
        private readonly Dictionary<IExport, ILifetimeContext> _global = new Dictionary<IExport, ILifetimeContext>();

        /// <summary>The _local.</summary>
        private readonly Dictionary<ExportMetadata, ILifetimeContext> _local =
            new Dictionary<ExportMetadata, ILifetimeContext>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="shareLifetime">
        ///     The share lifetime.
        /// </param>
        public void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime)
        {
            lock (this)
            {
                if (shareLifetime) _global[metadata.Export] = context;
                else _local[metadata] = context;
            }
        }

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="ILifetimeContext" />.
        /// </returns>
        public ILifetimeContext GetContext(ExportMetadata metadata)
        {
            lock (this)
            {
                ILifetimeContext context;
                if (metadata.Export.ShareLifetime) _global.TryGetValue(metadata.Export, out context);
                else _local.TryGetValue(metadata, out context);

                return context;
            }
        }

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            lock (this)
            {
                foreach (var disposable in
                    _global.Values.Concat(_local.Values)
                        .Select(lifetimeContext => lifetimeContext.GetValue())
                        .OfType<IDisposable>()) disposable.Dispose();

                _global.Clear();
                _local.Clear();
            }
        }

        /// <summary>
        ///     The free.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        //public void Free(ExportMetadata metadata)
        //{
        //    CContract.Requires<ArgumentNullException>(metadata != null, "metadata");

        //    lock (this)
        //    {
        //        ILifetimeContext con = GetContext(metadata);
        //        if (con == null) return;

        //        var disposable = con.GetValue() as IDisposable;
        //        if (disposable != null) disposable.Dispose();

        //        _global.Remove(metadata.Export);
        //        _local.Remove(metadata);
        //    }
        //}

        #endregion

        #region Methods

        /// <summary>The on clean up.</summary>
        private void OnCleanUp()
        {
            lock (this)
            {
                IEnumerable<IExport> deadKeysOne =
                    _global.Where(ent => ent.Value.IsAlive).Select(ent => ent.Key).ToArray();
                IEnumerable<ExportMetadata> deadkeysTwo =
                    (from ent in _local where ent.Value.IsAlive select ent.Key).ToArray();

                foreach (var export in deadKeysOne) _global.Remove(export);

                foreach (var exportMetadata in deadkeysTwo) _local.Remove(exportMetadata);
            }
        }

        #endregion
    }
}