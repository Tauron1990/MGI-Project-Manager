using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Anotar.Serilog;
using Tauron.Application.Deployment.Server.Engine;

namespace Tauron.Application.Deployment.Server.Services
{
    public sealed class SubscribeManager : IDisposable
    {
        private sealed class InternalRegistration
        {
            public BufferBlock<SyncError> Writer { get; }

            public InternalRegistration() => Writer = new BufferBlock<SyncError>();
        }

        private readonly ConcurrentDictionary<string, InternalRegistration> _registrations = new ConcurrentDictionary<string, InternalRegistration>();
        private readonly IPushMessager _messager;

        public SubscribeManager(IPushMessager messager)
        {
            _messager = messager;
            _messager.OnError += MessagerOnOnError;
        }

        private async Task MessagerOnOnError(SyncError arg)
        {
            foreach (var (key, internalRegistration) in _registrations)
            {
                try
                {
                    await internalRegistration.Writer.SendAsync(arg);
                }
                catch (Exception e)
                {
                    LogTo.Warning(e, "Error on Transfer ErrorInfo for: {Name}", key);
                    _registrations.TryRemove(key, out _);
                }
            }
        }

        public (bool OK, BufferBlock<SyncError> Block) Add(string name)
        {
            var registration = new InternalRegistration();
            return (_registrations.TryAdd(name, registration), registration.Writer);
        }

        public void Remove(string name)
        {
            if (_registrations.TryRemove(name, out var reg)) 
                reg.Writer.Complete();
        }
        public void Dispose()
        {
            _messager.OnError -= MessagerOnOnError;

            foreach (var internalRegistration in _registrations.Values) 
                internalRegistration.Writer.Complete();

            _registrations.Clear();
        }
    }
}