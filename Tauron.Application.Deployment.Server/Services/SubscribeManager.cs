using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Serilog;
using Grpc.Core;
using Tauron.Application.Deployment.Server.Engine;

namespace Tauron.Application.Deployment.Server.Services
{
    public sealed class SubscribeManager : IDisposable
    {
        private sealed class InternalRegistration
        {
            public CancellationTokenSource Token { get; }

            public IAsyncStreamWriter<SyncError> Writer { get; }

            public InternalRegistration(CancellationTokenSource token, IAsyncStreamWriter<SyncError> writer)
            {
                Token = token;
                Writer = writer;
            }
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
                    await internalRegistration.Writer.WriteAsync(arg);
                }
                catch (Exception e)
                {
                    LogTo.Warning(e, "Error on Transfer ErrorInfo for: {Name}", key);
                    _registrations.TryRemove(key, out _);
                }
            }
        }

        public bool Add(IAsyncStreamWriter<SyncError> writer, CancellationTokenSource token, string name) 
            => _registrations.TryAdd(name, new InternalRegistration(token, writer));

        public void Remove(string name)
        {
            if (_registrations.TryRemove(name, out var reg)) 
                reg.Token.Cancel();
        }
        public void Dispose() 
            => _messager.OnError -= MessagerOnOnError;
    }
}