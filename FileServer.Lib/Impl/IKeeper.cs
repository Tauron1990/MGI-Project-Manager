using System;

namespace FileServer.Lib.Impl
{
    public interface IKeeper<TType> : IDisposable
        where TType : class, IInternalOperation
    {
        TType Operation { get; }
    }
}