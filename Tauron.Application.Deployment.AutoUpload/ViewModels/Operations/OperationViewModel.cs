using System;
using System.Threading.Tasks;
using Catel.MVVM;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.Operations
{
    public abstract class OperationViewModel<TContext> : OperationViewModelBase
        where TContext : OperationContextBase
    {
        private TContext _context;

        #pragma warning disable CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
        protected OperationViewModel()
            #pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
        {
            CancelCommand = new Command(OnCancelCommandExecute, CanCancelExecute);
        }

        protected TContext Context
        {
            get
            {
                if (_context == null)
                    throw new InvalidOperationException("No Context is Setted");
                return _context;
            }
        }

        public Command CancelCommand { get; }

        protected virtual bool CanCancelExecute() => true;

        protected Task OnNextView<TType>(Redirection? redirection = null)
            where TType : OperationViewModel<TContext> =>
            OnNextView(typeof(TType), Context, redirection);

        protected virtual void OnCancelCommandExecute()
        {
            OnCancelOperation();
        }

        public override void SetContext(OperationContextBase contextBase)
        {
            if (contextBase is TContext context)
                _context = context;

            base.SetContext(contextBase);
        }

        protected Redirection CreateRedirection<TView>(RedirectionType redirectionType = RedirectionType.OnFinish)
            where TView : OperationViewModel<TContext> =>
            new Redirection(Context, typeof(TView), redirectionType);
    }
}