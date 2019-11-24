using System;
using System.Threading.Tasks;
using Catel.MVVM;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModelBase : ViewModelBase
    {
        public event Action? CancelOperation;

        public event Action<Type, OperationContextBase>? NextView;

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

        protected Task OnNextView<TType, TNewContext>(TNewContext newContext)
            where TType : OperationViewModel<TNewContext> where TNewContext : OperationContextBase
            => OnNextView(typeof(TType), newContext);

        protected async Task OnNextView(Type arg1, OperationContextBase arg2)
        {
            await CloseAsync();
            NextView?.Invoke(arg1, arg2);
        }

        protected Task Return()
            => OnNextView(typeof(CommandViewModel), OperationContextBase.Empty);

        public virtual void SetContext(OperationContextBase contextBase)
        {

        }
    }
}