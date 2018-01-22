using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class IOBusinessRuleBase<TInput, TOutput> : RuleBase, IIOBusinessRule<TInput, TOutput>
    {
        public virtual TOutput Action(TInput input)
        {
            try
            {
                return ActionImpl(input);
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                SetError(e);
                return default(TOutput);
            }
        }

        public abstract TOutput ActionImpl(TInput input);

        public override object GenericAction(object input)
        {
            return input == null ? Action(default(TInput)) : Action((TInput) input);
        }
    }
}