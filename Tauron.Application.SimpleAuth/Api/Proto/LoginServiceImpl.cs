using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Serilog.Context;
using Tauron.Application.Logging;
using Tauron.Application.SimpleAuth.Core;

namespace Tauron.Application.SimpleAuth.Api.Proto
{
    [Authorize(AuthenticationSchemes = "Simple")]
    public sealed class LoginServiceImpl : LoginService.LoginServiceBase
    {
        static LoginServiceImpl()
        {
            Result.Register<GetTokenResult>((result, status) => result.Status = status);
            Result.Register<SetPasswordResult>((result, status) => result.Status = status);
        }

        private readonly ISLogger<LoginV1Controller> _logger;
        private readonly IPasswordVault _passwordVault;
        private readonly ITokenManager _tokenManager;

        public LoginServiceImpl(IPasswordVault passwordVault, ITokenManager tokenManager, ISLogger<LoginV1Controller> logger)
        {
            _passwordVault = passwordVault;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        public override Task<GetTokenResult> GetToken(GetTokenData request, ServerCallContext context)
        {
            using (LogContext.PushProperty("Client", context.Peer))
            {
                try
                {
                    _logger.Information("Genrate Auth Token for: {Client}", context.Peer);

                    return Result.CreateAsync<GetTokenResult>(result => result.Token = _tokenManager.GenerateToken());
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error on generating Aut Token");

                    return Result.FailAsync<GetTokenResult>(e);
                }
            }
        }

        public override async Task<SetPasswordResult> Setpassword(NewPasswordData request, ServerCallContext context)
        {
            using (LogContext.PushProperty("Client", context.Peer))
            {
                try
                {
                    if (!await _passwordVault.CheckPassword(request.OldPassword))
                        return Result.Fail<SetPasswordResult>("Falsches Passwort angegeben");

                    if (!await _passwordVault.SetPassword(request.NewPassword))
                        return Result.Fail<SetPasswordResult>("Fehler beim setzen des Passwortes");

                    return Result.Create<SetPasswordResult>(d => d.Token = _tokenManager.GenerateToken());
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error on set Passwort");

                    return Result.Fail<SetPasswordResult>(e);
                }
            }
        }
    }
}