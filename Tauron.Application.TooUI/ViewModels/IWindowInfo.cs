using System.Windows;
using Catel.MVVM;

namespace Tauron.Application.TooUI.ViewModels
{
    public interface IWindowInfo : IViewModel
    {
        WindowState WindowState { get; }

        double Width { get; }

        double Height { get; }
    }
}