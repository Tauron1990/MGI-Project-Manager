using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModelBase : ViewModelBase
    {
        public event Action? CancelOperation;

        public event Action<Type, OperationContextBase>? NextView;

        private OperationContextBase? _currentContext;

        protected OperationViewModelBase()
        {
            DeferValidationUntilFirstSaveCall = false;
            ValidateModelsOnInitialization = true;
            AutomaticallyValidateOnPropertyChanged = true;
            DefaultValidateUsingDataAnnotationsValue = false;
        }

        protected async void OnCancelOperation()
        {
            await CloseAsync();
            CancelOperation?.Invoke();
        }

        protected Task OnNextView<TType, TNewContext>(TNewContext newContext, Redirection? redirection = null)
            where TType : OperationViewModel<TNewContext> where TNewContext : OperationContextBase
            => OnNextView(typeof(TType), newContext, redirection);

        protected async Task OnNextView(Type arg1, OperationContextBase arg2, Redirection? redirection = null)
        {
            await CloseAsync();

            var currentRedirection = arg2.Redirection;
            if (currentRedirection != null)
            {
                currentRedirection.ParentContext = _currentContext;
                currentRedirection.RedirectionContext.Redirection = redirection;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (currentRedirection.RedirectionType)
                {
                    case RedirectionType.FirstPage:
                        NextView?.Invoke(currentRedirection.RedirectionView, currentRedirection.RedirectionContext);
                        return;
                    case RedirectionType.OnFinish when typeof(CommonFinishViewModel) == arg1:
                        NextView?.Invoke(currentRedirection.RedirectionView, currentRedirection.RedirectionContext);
                        return;
                }
            }

            arg2.Redirection = redirection ?? currentRedirection;
            NextView?.Invoke(arg1, arg2);
        }


        protected async Task OnFinish(string? message = null)
        {
            await OnNextView<CommonFinishViewModel, FinishContext>(new FinishContext(message));
        }

        protected Task OnReturn()
            => OnNextView(typeof(CommandViewModel), OperationContextBase.Empty);

        public virtual void SetContext(OperationContextBase contextBase) 
            => _currentContext = contextBase;
    }
}