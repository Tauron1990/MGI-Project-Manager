using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JKang.IpcServiceFramework.Tcp
{
    public class TcpIpcServiceEndpoint<TContract> : IpcServiceEndpoint<TContract>
        where TContract : class
    {
        private readonly TcpListener _listener;
        private readonly ILogger<TcpIpcServiceEndpoint<TContract>> _logger;
        private readonly int _maximumConcurrentCalls;
        private readonly X509Certificate? _serverCertificate;
        private readonly Func<Stream, Stream>? _streamTranslator;

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, int port, TcpConcurrencyOptions? concurrencyOptions)
            : base(name, serviceProvider)
        {
            if (concurrencyOptions == null)
                _maximumConcurrentCalls = 1;
            else
            {
                _maximumConcurrentCalls = concurrencyOptions.MaximumConcurrentCalls;

                if (_maximumConcurrentCalls < 1)
                    _maximumConcurrentCalls = 1;
            }

            _listener = new TcpListener(ipEndpoint, port);
            _logger = serviceProvider.GetService<ILogger<TcpIpcServiceEndpoint<TContract>>>();
            Port = port;
        }

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, 0, concurrencyOptions)
        {
        }

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, Func<Stream, Stream> streamTranslator, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, 0, concurrencyOptions) =>
            _streamTranslator = streamTranslator;

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, X509Certificate sslCertificate, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, 0, concurrencyOptions)
        {
            _serverCertificate = sslCertificate;
            SSL = true;
        }

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, X509Certificate sslCertificate, Func<Stream, Stream> streamTranslator, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, sslCertificate, concurrencyOptions) =>
            _streamTranslator = streamTranslator;

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, int port, Func<Stream, Stream> streamTranslator, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, port, concurrencyOptions) =>
            _streamTranslator = streamTranslator;

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, int port, X509Certificate sslCertificate, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, port, concurrencyOptions)
        {
            _serverCertificate = sslCertificate;
            SSL = true;
        }

        public TcpIpcServiceEndpoint(string name, IServiceProvider serviceProvider, IPAddress ipEndpoint, int port, X509Certificate sslCertificate, Func<Stream, Stream> streamTranslator, TcpConcurrencyOptions? concurrencyOptions)
            : this(name, serviceProvider, ipEndpoint, port, sslCertificate, concurrencyOptions) =>
            _streamTranslator = streamTranslator;

        public int Port { get; private set; }

        public bool SSL { get; }

        public override Task ListenAsync(CancellationToken cancellationToken = default)
        {
            _listener.Start();

            // If port is dynamically assigned, get the port number after start
            Port = ((IPEndPoint) _listener.LocalEndpoint).Port;

            cancellationToken.Register(() => { _listener.Stop(); });

            return Task.Run(async () =>
            {
                try
                {
                    SemaphoreSlim? throttle = null;

                    if (_maximumConcurrentCalls > 1)
                        throttle = new SemaphoreSlim(_maximumConcurrentCalls);

                    _logger.LogDebug($"Endpoint '{Name}' listening on port {Port}...");

                    while (true)
                    {
                        if (throttle != null)
                            await throttle.WaitAsync(cancellationToken);

                        var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);

                        Stream server = client.GetStream();

                        // if there's a stream translator, apply it here
                        if (_streamTranslator != null) server = _streamTranslator(server);

                        // if SSL is enabled, wrap the stream in an SslStream in client mode
                        if (SSL)
                        {
                            var ssl = new SslStream(server, false);
                            await ssl.AuthenticateAsServerAsync(_serverCertificate);
                            server = ssl;
                        }

                        if (throttle == null)
                            await ProcessAsync(server, _logger, cancellationToken);
                        else
                        {
                            var processTask = Task.Run(
                                async () =>
                                {
                                    try
                                    {
                                        await ProcessAsync(server, _logger, cancellationToken).ConfigureAwait(false);
                                    }
                                    catch when (cancellationToken.IsCancellationRequested)
                                    {
                                    }
                                    finally
                                    {
                                        throttle.Release();
                                    }
                                }, cancellationToken);
                        }
                    }
                }
                catch when (cancellationToken.IsCancellationRequested)
                {
                }
            }, cancellationToken);
        }
    }
}