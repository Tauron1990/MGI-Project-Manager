using System;

namespace FileServer.Lib.Impl
{
    public interface IFileOperationManager
    {
        IKeeper<TOp>? Get<TOp>(Guid id)
            where TOp : class, IInternalOperation;

        bool Set<TOp>(TOp op, Guid id)
            where TOp : class, IInternalOperation;

        void Refresh(Guid id);
    }
}