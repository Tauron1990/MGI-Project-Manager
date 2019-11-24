using System;
using System.Threading.Tasks;
using Catel.MVVM;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModel<TContext> : OperationViewModelBase
        where TContext : OperationContextBase
    {
        private TContext? _context;

        public TContext Context
        {
            get
            {
                if(_context == null)
                    throw new InvalidOperationException("No Context is Setted");
                return _context;
            }
        }

        public Command CancelCommand { get; }

        protected OperationViewModel()
        {
            CancelCommand = new Command(OnCancelCommandExecute);
        }

        protected Task OnNextView<TType>()
            where TType : OperationViewModel<TContext>
            => OnNextView(typeof(TType), Context);

        protected virtual void OnCancelCommandExecute()
        {
            OnCancelOperation();
        }

        public override void SetContext(OperationContextBase contextBase)
        {
            if (contextBase is TContext context)
                _context = context;
        }
    }
}