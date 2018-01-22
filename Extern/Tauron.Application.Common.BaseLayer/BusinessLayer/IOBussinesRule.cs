namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public interface IOBussinesRule<out TOutput> : IRuleBase
    {
        TOutput Action();
    }
}