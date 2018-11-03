using System.Collections.Generic;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public sealed class CompositeRule<TInput, TOutput> : RuleBase, IBusinessRule, IIBusinessRule<TInput>, IIOBusinessRule<TInput, TOutput>
    {
        private readonly IEnumerable<IRuleBase> _rules;

        public CompositeRule(IEnumerable<IRuleBase> rules)
        {
            _rules = new ReadOnlyEnumerator<IRuleBase>(rules);
        }

        public override string InitializeMethod { get; } = nameof(Initialize);

        public override object GenericAction(object input)
        {
            return input == null ? Run(default) : Run((TInput) input);
        }

        void IBusinessRule.Action()
        {
            Run(default);
        }

        void IIBusinessRule<TInput>.Action(TInput input)
        {
            Run(input);
        }

        public TOutput Action(TInput input)
        {
            return Run(input);
        }

        public void Initialize(RepositoryFactory factory)
        {
            SetError(null);

            RepositoryFactory = factory;

            foreach (var rule in _rules) DatalayerHelper.InitializeRule(rule, factory);
        }

        private TOutput Run(TInput input)
        {
            using (var db = RepositoryFactory.EnterCompositeMode())
            {
                object output = input;
                var change = false;

                foreach (var ruleBase in _rules)
                {
                    var tempObj = ruleBase.GenericAction(output);
                    if (ruleBase.Error)
                    {
                        SetError(ruleBase.Errors);
                        return default;
                    }

                    if (tempObj == RuleNull.Null) continue;

                    output = tempObj;
                    change = true;
                }

                db.SaveChanges();

                if (!change || output == null) return default;

                return (TOutput) output;
            }
        }
    }
}