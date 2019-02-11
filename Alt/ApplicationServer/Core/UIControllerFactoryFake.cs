namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    internal sealed class UIControllerFactoryFake : IUIControllerFactory
    {
        public UIControllerFactoryFake()
        {
            SetSynchronizationContext();
        }

        public IUIController CreateController()
        {
            return new UIControllerFake();
        }

        public void SetSynchronizationContext()
        {
            UiSynchronize.Synchronize = new UISyncFake();
        }
    }
}