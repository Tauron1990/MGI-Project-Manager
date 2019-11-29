using System;
using System.Windows;

namespace Tauron.Application.Wpf.Helper
{
    public interface IControlBindable
    {
        IDisposable Bind(DependencyObject root, DependencyObject affectedObject, object dataContext);
        IDisposable NewContext(object newContext);
    }
}