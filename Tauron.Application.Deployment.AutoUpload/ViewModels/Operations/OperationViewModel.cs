using System;

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

        public override void SetContext(OperationContextBase contextBase)
        {
            if (contextBase is TContext context)
                _context = context;
        }
    }
}