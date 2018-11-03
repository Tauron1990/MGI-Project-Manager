using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Models
{
    [PublicAPI]
    [BaseTypeRequired(typeof(ViewModelBase))]
    public class ExportViewModelAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportViewModelAttribute([NotNull] string name) : base(typeof(ViewModelBase))
        {
            ContractName = name ?? throw new ArgumentNullException(nameof(name));
            IgnoreIntercepion = true;
        }

        public bool IgnoreIntercepion { get; set; }

        protected override bool HasMetadata => true;

        public override string DebugName => Name;

        protected override LifetimeContextAttribute OverrideDefaultPolicy => new NotSharedAttribute();

        public string Name => ContractName;
    }
}