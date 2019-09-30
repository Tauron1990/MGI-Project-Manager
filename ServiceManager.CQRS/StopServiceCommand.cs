using System;
using Tauron.CQRS.Services;

namespace ServiceManager.CQRS
{
    public class StopServiceCommand : IAmbientCommand
    {
        public string Name { get; set; }
    }
}
