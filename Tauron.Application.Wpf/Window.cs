using System;
using System.Windows;
using Catel.MVVM;
using Catel.Windows;
using Tauron.Application.Wpf.Helper;

namespace Tauron.Application.Wpf
{
    public class Window : DataWindow, IBinderControllable
    {
        private readonly ControlLogic _controlLogic;
        private readonly IViewModel _viewModel;

        protected Window(IViewModel viewModel)
            : base(viewModel)
        {
            _viewModel = viewModel;
            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _controlLogic = new ControlLogic(this, viewModel);
            DataContextChanged += (sender, args) =>
            {
                if (args.NewValue != _viewModel)
                    ((FrameworkElement) sender).DataContext = _viewModel;
            };
        }

        void IBinderControllable.Register(string key, IControlBindable bindable, DependencyObject affectedPart)
        {
            _controlLogic.Register(key, bindable, affectedPart);
        }

        public void CleanUp(string key)
        {
            _controlLogic.CleanUp(key);
        }

        protected override void OnUnloaded(EventArgs e)
        {
            _controlLogic.CleanUp();
        }
    }
}