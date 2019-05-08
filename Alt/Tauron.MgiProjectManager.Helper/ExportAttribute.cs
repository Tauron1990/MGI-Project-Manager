using System;
using JetBrains.Annotations;

namespace Tauron.MgiProjectManager
{
    [AttributeUsage(AttributeTargets.Class)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [PublicAPI]
    public class ExportAttribute : Attribute
    {
        [NotNull]
        public Type ExportType { get;}

        public bool CreateInstance { get; set; }

        [CanBeNull]
        public string Factory { get; set; }

        public LiveCycle LiveCycle { get; set; }

        public ExportAttribute([NotNull]Type exportType)
        {
            ExportType = exportType;
            LiveCycle = LiveCycle.Scoped;
        }
    }
}