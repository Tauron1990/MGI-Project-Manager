using System;
using Tauron.Application.Common.BaseLayer.BusinessLayer;

namespace Tauron.Application.Common.BaseLayer.Core
{
    public abstract class BusinessRuleBase : RuleBase, IBusinessRule
    {
        public virtual void Action()
        {
            try
            {
                ActionImpl();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                SetError(e);
            }
        }

        public override object GenericAction(object input)
        {
            Action();
            return RuleNull.Null;
        }

        public abstract void ActionImpl();
    }
}