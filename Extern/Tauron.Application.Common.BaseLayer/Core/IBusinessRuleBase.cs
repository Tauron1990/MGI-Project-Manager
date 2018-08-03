using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class IBusinessRuleBase<TType> : RuleBase, IIBusinessRule<TType>
    {
        public virtual void Action(TType input)
        {
            try
            {
                SetError(null);
                ActionImpl(input);
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                SetError(e);
            }
        }

        public override object GenericAction(object input)
        {
            if (input == null)
                Action(default(TType));
            Action((TType) input);

            return RuleNull.Null;
        }

        public abstract void ActionImpl(TType input);
    }
}