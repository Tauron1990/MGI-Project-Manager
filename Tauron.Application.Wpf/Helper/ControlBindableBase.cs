using System;
using System.Windows;

namespace Tauron.Application.Wpf.Helper
{
    public abstract class ControlBindableBase : IControlBindable
    {
        private class CleanUpHelper : IDisposable
        {
            private bool _isDisposed;
            private readonly ControlBindableBase _control;

            public CleanUpHelper(ControlBindableBase control) => _control = control;

            public void Dispose()
            {
                if(_isDisposed) return;

                _control.CleanUp();
                _isDisposed = true;
            }
        }

        private class Dummy : DependencyObject
        {
            
        }

        protected DependencyObject Root { get; private set; } = new Dummy();
        
        protected DependencyObject AffectedObject { get; private set; } = new Dummy();

        public IDisposable Bind(DependencyObject root, DependencyObject affectedObject, object dataContext)
        {
            Root = root;
            AffectedObject = affectedObject;
            Bind(dataContext);

            return new CleanUpHelper(this);
        }

        public IDisposable NewContext(object newContext)
        {
            Bind(newContext);
            return new CleanUpHelper(this);
        }

        protected abstract void CleanUp();

        protected abstract void Bind(object context);
    }
}