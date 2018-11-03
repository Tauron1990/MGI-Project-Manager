using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IRepositoryExtender
    {
        IDatabaseFactory DatabaseFactory { get; }
        IEnumerable<(Type, Type)> GetRepositoryTypes();
    }
}