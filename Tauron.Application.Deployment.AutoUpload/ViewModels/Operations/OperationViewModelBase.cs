using System;
using Catel.MVVM;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModelBase : ViewModelBase
    {
        public event Action? CancelOperation;

        public event Action<Type, OperationContextBase>? NextView;

        protected void OnCancelOperation() => CancelOperation?.Invoke();

        protected void OnNextView(Type arg1, OperationContextBase arg2) => NextView?.Invoke(arg1, arg2);

        public virtual void SetContext(OperationContextBase contextBase)
        {

        }
    }
}