using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CQRSlite.Commands;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using ServiceManager.Core;
using ServiceManager.CQRS;
using ServiceManager.Services;

namespace ServiceManager.ProcessManager
{
    public class ProcessManager : IProcessManager, IDisposable
    {
        private class ProcessHolder : IDisposable
        {
            private readonly AsyncLock _lock = new AsyncLock();
            private readonly Process _process;

            public ProcessHolder(Process process) => _process = process;

            public async Task<TType> Execute<TType>(Func<Process, Task<TType>> runner)
            {
                using (await _lock.LockAsync()) 
                    return await runner(_process);
            }

            public void Dispose() => _process?.Dispose();
        }

        private readonly ILogger<ProcessManager> _logger;
        private readonly ICommandSender _commandSender;
        private readonly ConcurrentDictionary<string, ProcessHolder> _processes = new ConcurrentDictionary<string, ProcessHolder>();

        public ProcessManager(ILogger<ProcessManager> logger, ICommandSender commandSender)
        {
            _logger = logger;
            _commandSender = commandSender;
        }

        public Task<bool> Start(RunningService service)
        {
            if(_processes.ContainsKey(service.Name)) return Task.FromResult(false);

            try
            {
                var process = Process.Start(Path.Combine(service.InstallationPath, service.Exe));

                _processes[service.Name] = new ProcessHolder(process);

                service.ServiceStade = ServiceStade.Running;

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{service.Name}: Error on Start Process");

                service.ServiceStade = ServiceStade.Error;
                return Task.FromResult(false);
            }
        }

        public async Task<bool> Stop(RunningService service, int timeToKill)
        {
            if (_processes.TryGetValue(service.Name, out var process))
            {
                return await process.Execute(async p =>
                {
                    bool error = false;

                    try
                    {
                        using var waiter = new ServiceStopWaiter(service.Name);
                        await _commandSender.Send(new StopServiceCommand {Name = service.Name});

                        await waiter.Wait(20_000);
                        if (p.WaitForExit(10000))
                            return true;
                        else
                        {
                            p.Kill();
                            return true;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        error = true;

                        _logger.LogError(e, $"{service.Name}: Invalid process Handle");
                        _processes.TryRemove(service.Name, out _);
                        return true;
                    }
                    catch (Exception e)
                    {
                        error = true;

                        _logger.LogError(e, $"{service.Name}: Error on Stop");
                        return false;
                    }
                    finally
                    {
                        service.ServiceStade = error ? ServiceStade.Error : ServiceStade.Ready;
                    }
                });
            }

            return false;
        }

        public void Dispose()
        {
            foreach (var process in _processes) 
                process.Value.Dispose();
        }
    }
}