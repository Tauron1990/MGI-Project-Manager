using JetBrains.Annotations;
using Tauron;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;

namespace TestHelperLib
{
    [PublicAPI]
    public static class RuleHelper
    {
        public static IBusinessRule GetBusinessRule(this IContainer container, string name) => GetRuleFactory(container).CreateBusinessRule(name);

        public static IIBusinessRule<TInput> GetIBusinessRule<TInput>(this IContainer container, string name) => GetRuleFactory(container).CreateIiBusinessRule<TInput>(name);

        public static IOBussinesRule<TOutput> GetOBusinessRule<TOutput>(this IContainer container, string name) => GetRuleFactory(container).CreateOBussinesRule<TOutput>(name);

        public static IIOBusinessRule<TInput, TOutput> GetIoBusinessRule<TInput, TOutput>(this IContainer container, string name) => GetRuleFactory(container).CreateIioBusinessRule<TInput, TOutput>(name);

        private static RuleFactory GetRuleFactory(IContainer container) => container.Resolve<RuleFactory>();
    }
}
