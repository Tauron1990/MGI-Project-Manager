using System;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IDatabaseIdentifer : IDisposable
    {
        string Id { get; }
    }
}