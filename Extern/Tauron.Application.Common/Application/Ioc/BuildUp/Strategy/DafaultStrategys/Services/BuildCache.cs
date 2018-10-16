#region

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
                var toDispose = new List<IDisposable>();
                toDispose.AddRange(_global.Where(p => !p.Key.ExternalInfo.HandlesDispose)
                    .Select(p => p.Value?.GetValue()).OfType<IDisposable>());
                toDispose.AddRange(_local.Where(p => !p.Key.Export.ExternalInfo.HandlesDispose)
                    .Select(p => p.Value?.GetValue()).OfType<IDisposable>());

                foreach (var disposable in toDispose)
                    disposable.Dispose();

                _global.Clear();
                _local.Clear();
            }
        }

        #endregion
    }
}