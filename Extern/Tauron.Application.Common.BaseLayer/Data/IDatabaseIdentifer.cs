using System;
using Microsoft.EntityFrameworkCore;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IDatabaseIdentifer : IDisposable
    {
        string Id { get; }

        DbContext Context { get; }
    }
}