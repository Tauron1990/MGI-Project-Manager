using Catel.MVVM;
using Microsoft.Extensions.Hosting;
using Tauron.Application.TooUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.TooUI
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly IWpfLifetime _lifetime;

        public MainWindowViewModel(IWpfLifetime lifetime) 
            => _lifetime = lifetime;


        public IWindowInfo? MainContent { get; set; }

        [CommandTarget]
        public void CloseApp() 
            => _lifetime.Shutdown();
    }
}