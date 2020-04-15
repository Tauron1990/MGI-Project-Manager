using System;

namespace Tauron.Application.Wpf.AppCore
{
    public sealed class DelegateAppFactory : IAppFactory
    {
        private readonly Func<System.Windows.Application> _creator;

        public DelegateAppFactory(Func<System.Windows.Application> creator) 
            => _creator = creator;

        public System.Windows.Application Create() => _creator();
    }
}