using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [PublicAPI]
    public interface IIOBusinessRule<in TInput, out TOutput> : IRuleBase
        //where TInput : class
        //where TOutput : class
    {
        TOutput Action(TInput input);
    }
}