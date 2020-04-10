namespace Tauron.Application.Wpf.AppCore
{
    public sealed class SimpleSplashScreen<TSplash> : ISplashScreen
        where TSplash : System.Windows.Window, new()
    {
        public System.Windows.Window Window => new TSplash();
    }
}