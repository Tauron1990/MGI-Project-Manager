using System;

namespace FileServer.Lib.Impl
{
    public abstract class OperationBase : IInternalOperation
    {
        public void Dispose()
        {
            if(IsCollected) return;

            DisposeImpl();
            IsCollected = true;
        }

        
        public bool IsCollected { get; private set; }
        public abstract TimeSpan Timeout { get; }

        public void Compled()
        {
            if(IsCollected) return;

            FinishOperation();
            Dispose();
        }

        protected abstract void DisposeImpl();

        protected abstract void FinishOperation();
    }
}