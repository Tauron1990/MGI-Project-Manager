using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.BL.Impl;

namespace Tauron.Application.MgiProjectManager.BL
{
    public static class ServiceExtensions
    {
        public static void AddBLServices(this IServiceCollection collection)
        {
            collection.AddSingleton<ITimedTaskManager, TimedTaskManager>();
            collection.AddSingleton<IOperationManager, OperationManager>();
            collection.AddSingleton<IFileManager, FileManager>();
        }
    }
}