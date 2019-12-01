﻿using System;
using System.Windows;
using Catel.MVVM;
using Tauron.Application.Wpf.Helper;

namespace Tauron.Application.Wpf
{
    public class UserControl : Catel.Windows.Controls.UserControl, IBinderControllable
    {
        private readonly IViewModel _viewModel;
        private readonly ControlLogic _controlLogic;

        protected UserControl(IViewModel viewModel)
            : base(viewModel)
        {
            _viewModel = viewModel;
            _controlLogic = new ControlLogic(this, viewModel);
            DataContextChanged += (sender, args) =>
                                  {
                                      if (args.NewValue != _viewModel)
                                          ((FrameworkElement) sender).DataContext = _viewModel;
                                  };
        }

        protected override void OnUnloaded(EventArgs e) 
            => _controlLogic.CleanUp();

        void IBinderControllable.Register(string key, IControlBindable bindable, DependencyObject affectedPart)
        {
            _controlLogic.Register(key, bindable, affectedPart);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        public void CleanUp(string key) 
            => _controlLogic.CleanUp(key);
    }
}