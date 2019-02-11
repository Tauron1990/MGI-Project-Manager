namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    internal sealed class UIControllerFake : IUIController
    {
        public IWindow      MainWindow   { get; set; }
        public ShutdownMode ShutdownMode { get; set; }

        public void Run(IWindow window)
        {
        }

        public void Shutdown()
        {
        }
    }
}