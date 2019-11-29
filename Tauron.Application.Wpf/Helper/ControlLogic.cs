using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Tauron.Application.Wpf.Helper
{
    public sealed class ControlLogic
    {
        private readonly DependencyObject _target;
        private object? _dataContext;

        private readonly Dictionary<string, (IDisposable Disposer, IControlBindable Binder)> _binderList = new Dictionary<string, (IDisposable Disposer, IControlBindable Binder)>();

        public ControlLogic(DependencyObject target, object dataContext)
        {
            _target = target;
            _dataContext = dataContext;
        }

        public void NewDataContext(object? dataContext)
        {
            _dataContext = dataContext;

            foreach (var (key, (disposer, binder)) in _binderList.ToArray())
            {
                disposer.Dispose();
                if (dataContext != null)
                    _binderList[key] = (binder.NewContext(dataContext), binder);
            }
        }

        public void CleanUp()
        {
            foreach (var pair in _binderList) 
                pair.Value.Disposer.Dispose();

            _binderList.Clear();
        }

        public void Register(string key, IControlBindable bindable, DependencyObject affectedPart)
        {
            if(_dataContext == null)
                return;

            var disposer = bindable.Bind(_target, affectedPart, _dataContext);

            _binderList[key] = (disposer, bindable);
        }

        public void CleanUp(string key)
        {
            if(_binderList.TryGetValue(key, out var pair))
                pair.Disposer.Dispose();

            _binderList.Remove(key);
        }
    }
}