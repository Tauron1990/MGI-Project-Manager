using System;
using System.ServiceModel;

namespace Tauron.Application.ProjectManager.UI
{
    public interface IClientHelperBase : ICommunicationObject, IDisposable
    {
        string Password { get; set; }
        string Name     { get; set; }
    }

    public interface ITypedClientHelperBase<out TClient> : IClientHelperBase 
        where TClient : class
    {
        TClient Client { get; }
    }
}