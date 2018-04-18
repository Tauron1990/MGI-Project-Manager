using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.BaseLayer
{
    [BaseTypeRequired(typeof(IRuleBase))]
    public sealed class ExportRuleAttribute : ExportAttribute, IRuleMetadata
    {
        public ExportRuleAttribute(string name)
            : base(typeof(IRuleBase))
        {
            Name = name;
        }

        protected override LifetimeContextAttribute OverrideDefaultPolicy { get; } = new NotSharedAttribute();

        protected override bool HasMetadata => true;

        public string Name { get; }
    }
}