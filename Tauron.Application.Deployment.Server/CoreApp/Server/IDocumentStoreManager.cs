using System;
using Raven.Client.Documents;

namespace Tauron.Application.Deployment.Server.CoreApp.Server
{
    public interface IDocumentStoreManager : IDisposable
    {
        IDocumentStore Get(string name);
    }
}