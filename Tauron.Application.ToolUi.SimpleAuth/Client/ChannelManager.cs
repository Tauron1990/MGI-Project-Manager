using System;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Serilog.Extensions.Logging;
using Tauron.Application.SimpleAuth.Api.Proto;
using Tauron.Application.ToolUI.Login;

namespace Tauron.Application.ToolUi.SimpleAuth.Client
{
    [ServiceDescriptor(typeof(ChannelManager), ServiceLifetime.Scoped)]
    public sealed class ChannelManager : IDisposable
    {
        private sealed class TokenInfo
        {
            public DateTime Creation { get; } = DateTime.Now + TimeSpan.FromHours(20);

            public string Token { get; }

            public TokenInfo(string token) => Token = token;
        }

        private readonly InputService _inputService;
        private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new ConcurrentDictionary<string, GrpcChannel>();
        private readonly ConcurrentDictionary<string, TokenInfo> _tokens = new ConcurrentDictionary<string, TokenInfo>();

        public ChannelManager(InputService inputService) 
            => _inputService = inputService;

        public GrpcChannel Channel(string host)
        {
            return _channels.GetOrAdd(host, h =>
            {
                var credinals = CallCredentials.FromInterceptor((c, m) => AuthClient(c, m, h));
                return GrpcChannel.ForAddress(new Uri(h), new GrpcChannelOptions
                                                             {
                                                                 Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credinals),
                                                                 LoggerFactory = new SerilogLoggerFactory()

                                                             });
            });
        }



        private async Task AuthClient(AuthInterceptorContext context, Metadata metadata, string host)
        {
            if(context.MethodName == nameof(LoginService.LoginServiceClient.Setpassword))
                return;

            AuthenticationHeaderValue TokenHeader(TokenInfo token)
            {
                var parm = $"token:{token.Token}";
                return ConvertToHeader(parm);
            }

            AuthenticationHeaderValue? header;

            if (_tokens.TryGetValue(host, out var tokenInfo) && tokenInfo.Creation > DateTime.Now)
            {
                header = TokenHeader(tokenInfo);
            }
            else if(context.MethodName == "GetToken")
            {
                var pass = await _inputService.Request("Passwort", "für Service");
                header = ConvertToHeader($"pass:{pass}");
            }
            else
            {
                var client = new LoginService.LoginServiceClient(Channel(host));
                var token = await client.GetTokenAsync(new GetTokenData());
                if (token.ResultCase == GetTokenResult.ResultOneofCase.Token)
                {
                    tokenInfo = new TokenInfo(token.Token);
                    header = TokenHeader(tokenInfo);

                    _tokens[host] = tokenInfo;
                }
                else
                    throw new RpcException(new Status((StatusCode)token.Status.Code, token.Status.Message));
            }

            metadata.Add("Authorization", header.ToString());
        }

        private AuthenticationHeaderValue ConvertToHeader(string str)
            => new AuthenticationHeaderValue("Simple", Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));

        public void Dispose()
        {
            foreach (var channel in _channels.Values)
            {
                channel?.ShutdownAsync().Wait();
                channel?.Dispose();
            }

            _channels.Clear();
        }
    }
}