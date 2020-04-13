using System;
using System.Threading.Tasks;
using Anotar.Serilog;
using Catel.MVVM;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModelBase : ViewModelBase
    {
        private OperationContextBase? _currentContext;

        protected OperationViewModelBase()
        {
            DeferValidationUntilFirstSaveCall = false;
            ValidateModelsOnInitialization = true;
            AutomaticallyValidateOnPropertyChanged = true;
            DefaultValidateUsingDataAnnotationsValue = false;
        }

        public event Action? CancelOperation;

        public event Action<Type, OperationContextBase>? NextView;

        protected async void OnCancelOperation()
        {
            await CloseAsync();
            CancelOperation?.Invoke();
        }

        protected Task OnNextView<TType, TNewContext>(TNewContext newContext, Redirection? redirection = null)
            where TType : OperationViewModel<TNewContext> where TNewContext : OperationContextBase =>
            OnNextView(typeof(TType), newContext, redirection);

        protected async Task OnNextView(Type arg1, OperationContextBase arg2, Redirection? redirection = null)
        {
            LogTo.Information("Next View: {View}", arg1);
            await CloseAsync();

            var currentRedirection = arg2.Redirection;
            if (currentRedirection != null)
            {
                currentRedirection.RedirectionContext.Redirection = redirection;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (currentRedirection.RedirectionType)
                {
                    case RedirectionType.FirstPage:
                        (currentRedirection.RedirectionContext as IContextApply)?.Apply(currentRedirection.RedirectionContext);
                        NextView?.Invoke(currentRedirection.RedirectionView, currentRedirection.RedirectionContext);
                        return;
                }
            }

            arg2.Redirection = redirection ?? currentRedirection;
            NextView?.Invoke(arg1, arg2);
        }


        protected async Task OnFinish(string? message = null)
        {
            var currentRedirection = _currentContext?.Redirection;
            if (currentRedirection != null)
            {
                if (currentRedirection.RedirectionType == RedirectionType.OnFinish)
                {
                    if (_currentContext != null)
                        (currentRedirection.RedirectionContext as IContextApply)?.Apply(_currentContext);
                    NextView?.Invoke(currentRedirection.RedirectionView, currentRedirection.RedirectionContext);
                    return;
                }
            }

            await OnNextView<CommonFinishViewModel, FinishContext>(new FinishContext(message));
        }

        protected Task OnReturn() => OnNextView(typeof(CommandViewModel), OperationContextBase.Empty);

        public virtual void SetContext(OperationContextBase contextBase)
        {
            _currentContext = contextBase;
        }
    }
}