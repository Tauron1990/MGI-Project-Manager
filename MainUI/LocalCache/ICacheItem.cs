using System;

namespace Tauron.Application.MgiProjectManager.LocalCache
{
    public interface ICacheItem
    {
        Type IdentiferType { get; }

        object Content { get; }
    }
}