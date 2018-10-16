using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Models
{
    [PublicAPI]
    [BaseTypeRequired(typeof(IModel))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ExportModelAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportModelAttribute([NotNull] string name)
            : base(typeof(IModel))
        {
            ContractName = name ?? throw new ArgumentNullException(nameof(name));
        }

        protected override bool HasMetadata => true;

        public string Name => ContractName;
    }
}