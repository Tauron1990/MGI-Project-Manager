using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [PublicAPI]
    public interface IIBusinessRule<in TInput> : IRuleBase
    {
        void Action(TInput input);
    }
}