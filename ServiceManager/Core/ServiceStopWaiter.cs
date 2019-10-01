using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using ServiceManager.CQRS;

namespace ServiceManager.Core
{
    public class ServiceStopWaiter: IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly AsyncManualResetEvent _manualReset = new AsyncManualResetEvent(false);
        private readonly string _targetService;

        public ServiceStopWaiter(string targetService)
        {
            _targetService = targetService;
            ServiceStopHandler.ServiceStoppedEvent += ServiceStopHandlerOnServiceStoppedEvent;
        }

        private Task ServiceStopHandlerOnServiceStoppedEvent(ServiceStoppedEvent arg)
        {
            if(arg.ServiceName == _targetService) _manualReset.Set();

            return Task.CompletedTask;
        }

        public Task Wait(int timeout)
        {
            _cancellationTokenSource = new CancellationTokenSource(timeout);
            return _manualReset.WaitAsync(_cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            ServiceStopHandler.ServiceStoppedEvent -= ServiceStopHandlerOnServiceStoppedEvent;
        }
    }
}