using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.CoreApp.Server;

namespace Tauron.Application.Deployment.Server.CoreApp.Bridge
{
    public interface IServerBridge
    {
        IClientSetup ClientSetup { get; }
    }
}
