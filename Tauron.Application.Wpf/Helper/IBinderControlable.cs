using System.Windows;

namespace Tauron.Application.Wpf.Helper
{
    public interface IBinderControllable
    {
        void Register(string key, IControlBindable bindable, DependencyObject affectedPart);
        void CleanUp(string key);
    }
}