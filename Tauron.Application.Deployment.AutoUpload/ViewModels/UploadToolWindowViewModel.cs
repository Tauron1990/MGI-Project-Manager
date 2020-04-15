using System;
using System.Windows;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Logging;
using Tauron.Application.ToolUI.ViewModels;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor(typeof(UploadToolWindowViewModel), ServiceLifetime.Scoped)]
    [LogContextProvider(typeof(UploadLogContext))]
    public sealed class UploadToolWindowViewModel : ViewModelBase, IToolWindow
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISLogger<UploadToolWindowViewModel> _logger;

        //[NoWeaving]
        //public BusyService BusyService { get; }

        public UploadToolWindowViewModel(IServiceProvider serviceProvider, ISLogger<UploadToolWindowViewModel> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            Title = "Auto Versionen und Uploads";
            //BusyService = (BusyService) pleaseWaitService;

            ModelOnCancelOperation();
        }


        public OperationViewModelBase? CurrentView { get; set; }

        private void ChangeView(OperationViewModelBase model)
        {
            _logger.Information("Change Upload View to {ViewModel}", model.GetType());

            model.CancelOperation += ModelOnCancelOperation;
            model.NextView += ModelOnNextView;

            if (CurrentView != null)
            {
                CurrentView.CancelOperation -= ModelOnCancelOperation;
                CurrentView.NextView -= ModelOnNextView;
            }

            CurrentView = model;
        }

        private void ModelOnNextView(Type obj, OperationContextBase operationContext)
        {
            var model = _serviceProvider.GetService(obj);
            if (model is OperationViewModelBase operationView)
            {
                operationView.SetContext(operationContext);
                ChangeView(operationView);
            }
            else
                ModelOnCancelOperation();
        }

        private void ModelOnCancelOperation()
        {
            //ModelOnNextView(typeof(BuildVersionIncrementViewModel), new BuildOperationContext(new BuildContext()));
            ChangeView(_serviceProvider.GetRequiredService<CommandViewModel>());
        }

        public SizeToContent SizeToContent => SizeToContent.Manual;
        public double Width => 800;
        public double Height => 450;
    }
}