using System;

namespace Tauron.Application.Wpf.AppCore
{
    public interface IMainWindow : IWindowProvider
    {
        event EventHandler Shutdown;
    }
}