using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [Export(typeof(RuleFactory))]
    [PublicAPI]
    public sealed class RuleFactory
    {
        private readonly IDictionary<string, IRuleBase> _cache = new Dictionary<string, IRuleBase>();

        [InjectRepositoryFactory]
        private RepositoryFactory _repositoryFactory;

        [Inject(typeof(IRuleBase))]
        private InstanceResolver<IRuleBase, IRuleMetadata>[] _rules;

        private IRuleBase GetOrCreate(string name)
        {
            if (_cache.TryGetValue(name, out var cRule)) return cRule;

            var rule = _rules.Single(i => i.Metadata.Name == name).Resolve();

            _cache[name] = rule;

            return rule;
        }

        public IRuleBase Create(string name)
        {
            var rule = GetOrCreate(name);

            DatalayerHelper.InitializeRule(rule, _repositoryFactory);

            return rule;
        }

        public IBusinessRule CreateBusinessRule(string name)
        {
            return (IBusinessRule) Create(name);
        }

        public IIBusinessRule<TType> CreateIiBusinessRule<TType>(string name)
        {
            return (IIBusinessRule<TType>) Create(name);
        }

        public IIOBusinessRule<TInput, TOutput> CreateIioBusinessRule<TInput, TOutput>(string name)
            where TOutput : class where TInput : class
        {
            return (IIOBusinessRule<TInput, TOutput>) Create(name);
        }

        public IOBussinesRule<TOutput> CreateOBussinesRule<TOutput>(string name)
            where TOutput : class
        {
            return (IOBussinesRule<TOutput>) Create(name);
        }

        public CompositeRule<TInput, TOutput> CreateComposite<TInput, TOutput>(params string[] names)
        {
            var keyBuilder = new StringBuilder();

            foreach (var name in names)
            {
                keyBuilder.Append(name);
            }

            var                            key = keyBuilder.ToString();
            CompositeRule<TInput, TOutput> compositeule;

            if (_cache.TryGetValue(key, out var cache))
            {
                compositeule = (CompositeRule<TInput, TOutput>) cache;
            }
            else
            {
                compositeule = new CompositeRule<TInput, TOutput>(names.Select(GetOrCreate));
                _cache[key]  = compositeule;
            }

            DatalayerHelper.InitializeRule(compositeule, _repositoryFactory);
            return compositeule;
        }
    }
}