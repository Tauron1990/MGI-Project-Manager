using System.Windows;
using Catel.MVVM;

namespace Tauron.Application.ToolUI.ViewModels
{
    public interface IToolWindow : IViewModel
    {
        public WindowState WindowState => WindowState.Normal;

        SizeToContent SizeToContent { get; }

        double Width { get; }

        double Height { get; }
    }
}