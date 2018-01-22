using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.Components
{
    [PublicAPI]
    public class RebuildManager
    {
        #region Fields

        private readonly GroupDictionary<ExportMetadata, BuildObject> _objects =
            new GroupDictionary<ExportMetadata, BuildObject>(typeof (WeakReferenceCollection<BuildObject>));

        #endregion


        #region Public Methods and Operators

        public void AddBuild([NotNull] BuildObject instance)
        {
            Contract.Requires<ArgumentNullException>(instance != null, "instance");

            lock (this)
            {
                _objects[instance.Metadata].Add(instance);
            }
        }

        [NotNull]
        public IEnumerable<BuildObject> GetAffectedParts([NotNull] IEnumerable<ExportMetadata> added,
                                                         [NotNull] IEnumerable<ExportMetadata> removed)
        {
            Contract.Requires<ArgumentNullException>(added != null, "added");
            Contract.Requires<ArgumentNullException>(removed != null, "removed");

            lock (this)
            {
                IEnumerable<ExportMetadata> changed = added.Concat(removed);

                return from o in _objects
                       from buildObject in o.Value
                       where
                           buildObject.GetImports()
                                      .Any(
                                          tup =>
                                          changed.Any(
                                              meta =>
                                              tup.InterfaceType == meta.InterfaceType
                                              && tup.ContractName == meta.ContractName))
                       where buildObject.IsAlive
                       select buildObject;
            }
        }

        #endregion
    }
}