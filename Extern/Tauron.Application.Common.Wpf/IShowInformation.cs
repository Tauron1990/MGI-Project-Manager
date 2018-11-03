namespace Tauron.Application
{
    public interface IShowInformation
    {
        void OnShow(IWindow window);
        void AfterShow(IWindow window);
    }
}