#region

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The build object.</summary>
    [PublicAPI]
    public class BuildObject : INotifyIsAlive
    {
        #region Fields

        private readonly ImportMetadata[] _imports;

        /// <summary>The _instance.</summary>
        private WeakReference _instance;

        #endregion

        #region Constructors and Destructors

        public BuildObject([NotNull] IEnumerable<ImportMetadata> imports, [NotNull] ExportMetadata targetExport, [CanBeNull]BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(imports != null, "imports");
            Contract.Requires<ArgumentNullException>(targetExport != null, "targetExport");

            Metadata = targetExport;
            _imports = imports.ToArray();
            Export = targetExport.Export;
            BuildParameters = buildParameters;
        }

        #endregion

        #region Public Properties

        [NotNull]
        public IExport Export { get; private set; }

        [CanBeNull]
        public object Instance
        {
            get { return _instance.Target; }

            set { _instance = new WeakReference(value); }
        }

        [NotNull]
        public ExportMetadata Metadata { get; set; }

        public bool IsAlive
        {
            get { return _instance.IsAlive; }
        }

        [CanBeNull]
        public BuildParameter[] BuildParameters { get; set; }

        #endregion

        #region Public Methods and Operators

        [NotNull]
        public ImportMetadata[] GetImports()
        {
            Contract.Ensures(Contract.Result<ImportMetadata[]>() != null);

            return (ImportMetadata[]) _imports.Clone();
        }

        #endregion
    }
}