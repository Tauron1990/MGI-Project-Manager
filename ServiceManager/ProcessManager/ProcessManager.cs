using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using ServiceManager.Services;

namespace ServiceManager.ProcessManager
{
    public class ProcessManager : IProcessManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, Process> _processes = new ConcurrentDictionary<string, Process>();

        public async Task Start(RunningService service)
        {
            if (_processes.TryGetValue(service.Name, out var process))
            {
                lock (process)
                {
                    try
                    {
                        process.
                    }
                    catch (InvalidOperationException)
                    {
                        _processes.TryRemove(service.Name, out _);
                    }
                    finally
                    {
                        service.ServiceStade = ServiceStade.Ready;
                    }
                }
            }
        }

        public async Task Stop(RunningService service, int timeToKill) => throw new System.NotImplementedException();

        public void Dispose()
        {
            foreach (var process in _processes) 
                process.Value.Dispose();
        }
    }
}