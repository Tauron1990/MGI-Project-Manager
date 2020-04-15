using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JKang.IpcServiceFramework.Services;

namespace JKang.IpcServiceFramework.Tcp
{
    internal class TcpIpcServiceClient<TInterface> : IpcServiceClient<TInterface>
        where TInterface : class
    {
        private readonly IPAddress _serverIp;
        private readonly int _serverPort;
        private readonly string? _sslServerIdentity;
        private readonly RemoteCertificateValidationCallback? _sslValidationCallback;
        private readonly Func<Stream, Stream>? _streamTranslator;

        public TcpIpcServiceClient(IIpcMessageSerializer serializer, IValueConverter converter, IPAddress serverIp, int serverPort)
            : base(serializer, converter)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            SSL = false;
        }

        public TcpIpcServiceClient(IIpcMessageSerializer serializer, IValueConverter converter, IPAddress serverIp, int serverPort, Func<Stream, Stream> streamTranslator)
            : this(serializer, converter, serverIp, serverPort) =>
            _streamTranslator = streamTranslator;

        public TcpIpcServiceClient(IIpcMessageSerializer serializer, IValueConverter converter, IPAddress serverIp, int serverPort, string sslServerIdentity, RemoteCertificateValidationCallback? sslCertificateValidationCallback = null)
            : this(serializer, converter, serverIp, serverPort)
        {
            _sslValidationCallback = sslCertificateValidationCallback;
            _sslServerIdentity = sslServerIdentity;
            SSL = true;
        }

        public TcpIpcServiceClient(IIpcMessageSerializer serializer, IValueConverter converter, IPAddress serverIp, int serverPort, string sslServerIdentity, Func<Stream, Stream> streamTranslator)
            : this(serializer, converter, serverIp, serverPort, sslServerIdentity) =>
            _streamTranslator = streamTranslator;

        public TcpIpcServiceClient(IIpcMessageSerializer serializer, IValueConverter converter, IPAddress serverIp, int serverPort, string sslServerIdentity, RemoteCertificateValidationCallback sslCertificateValidationCallback, Func<Stream, Stream> streamTranslator)
            : this(serializer, converter, serverIp, serverPort, sslServerIdentity, sslCertificateValidationCallback) =>
            _streamTranslator = streamTranslator;


        public bool SSL { get; }

        protected override async Task<Stream> ConnectToServerAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = new TcpClient();
            var result = client.BeginConnect(_serverIp, _serverPort, null, null);

            await Task.Run(() =>
            {
                // poll every 100ms to check cancellation request
                while (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(100), false))
                {
                    if (!cancellationToken.IsCancellationRequested) continue;

                    client.EndConnect(result);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }, cancellationToken).ConfigureAwait(false);

            cancellationToken.Register(() => { client.Close(); });

            Stream stream = client.GetStream();

            // if there's a stream translator, apply it here
            if (_streamTranslator != null) stream = _streamTranslator(stream);

            // if SSL is enabled, wrap the stream in an SslStream in client mode
            if (!SSL) return stream;
            
            var ssl = _sslValidationCallback == null ? new SslStream(stream, false) : new SslStream(stream, false, _sslValidationCallback);

            // set client mode and specify the common name(CN) of the server
            await ssl.AuthenticateAsClientAsync(_sslServerIdentity);
            stream = ssl;

            return stream;
        }
    }
}