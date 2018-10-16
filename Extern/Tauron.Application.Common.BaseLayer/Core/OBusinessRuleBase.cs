using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class OBusinessRuleBase<TOutput> : RuleBase, IOBussinesRule<TOutput>
    {
        public virtual TOutput Action()
        {
            try
            {
                SetError(null);
                return ActionImpl();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                SetError(e);
                return default(TOutput);
            }
        }

        public override object GenericAction(object input)
        {
            return Action();
        }

        public abstract TOutput ActionImpl();
    }
}