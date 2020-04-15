using System;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class WpfLifetime : IWpfLifetime
    {
        public event EventHandler? ShutdownEvent;
        
        public void Shutdown() 
            => ShutdownEvent?.Invoke(this, EventArgs.Empty);
    }
}