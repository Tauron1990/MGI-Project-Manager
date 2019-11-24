using System;
using Catel.Fody;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels
{
    [ServiceDescriptor(typeof(MainWindowViewModel), ServiceLifetime.Singleton)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;

        [NoWeaving]
        public BusyService BusyService { get; }

        public MainWindowViewModel(IPleaseWaitService pleaseWaitService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Title = "Auto Versionen und Uploads";
            BusyService = (BusyService) pleaseWaitService;

            ModelOnCancelOperation();
        }


        public OperationViewModelBase? CurrentView { get; set; }

        private void ChangeView(OperationViewModelBase model)
        {
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
            => ChangeView(_serviceProvider.GetRequiredService<CommandViewModel>());
    }
}