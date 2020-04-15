using System;

namespace Tauron.Application.Wpf
{

    public interface IWpfLifetime
    {
        event EventHandler ShutdownEvent;

        void Shutdown();
    }
}