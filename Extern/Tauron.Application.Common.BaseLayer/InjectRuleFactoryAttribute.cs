using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer
{
    public class InjectRuleFactoryAttribute : InjectAttribute
    {
        public InjectRuleFactoryAttribute()
            : base(typeof(RuleFactory))
        {
        }
    }
}