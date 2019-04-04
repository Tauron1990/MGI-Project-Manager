using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.MgiProjectManager.Server.Data.Impl;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.Server.Data
{
    public static class ServiceExtensions
    {
        public static void AddDBServices(this IServiceCollection collection)
        {
            collection.AddSingleton<ITimedTaskRepository, TimedTaskRepository>();
            collection.AddSingleton<IOperationRepository, OperationRepository>();
        }
    }
}