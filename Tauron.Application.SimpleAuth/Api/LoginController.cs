using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using Tauron.Application.Logging;
using Tauron.Application.SimpleAuth.Core;
using Tauron.Application.SimpleAuth.Data;

namespace Tauron.Application.SimpleAuth.Api
{
    /// <summery>
    ///     Gibt ein 24 Stunden gültiges Token für die Authentifizierung zurück.
    ///     Auserdem kann das Password geändert werden.
    /// </summery>
    [Route("api/Login/V1")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Simple")]
    public class LoginV1Controller : ControllerBase
    {
        private readonly ISLogger<LoginV1Controller> _logger;
        private readonly IPasswordVault _passwordVault;
        private readonly ITokenManager _tokenManager;


        public LoginV1Controller(IPasswordVault passwordVault, ITokenManager tokenManager, ISLogger<LoginV1Controller> logger)
        {
            _passwordVault = passwordVault;
            _tokenManager = tokenManager;
            _logger = logger;
        }

        /// <summary>
        ///     Erzeut ein 24 Stunden gültiges token.
        /// </summary>
        /// <returns>Das Token zur Authentifizierung</returns>
        /// <response code="201">Gibt das Token zurück</response>
        [HttpGet("Token")]
        [ProducesResponseType(typeof(GetTokenResult), (int) HttpStatusCode.Created)]
        public GetTokenResult GetToken()
        {
            LogContext.PushProperty("Client", HttpContext.Connection.Id);

            try
            {
                _logger.Information("Genrate Auth Token for: {Client}", HttpContext.Connection.Id);
                return GetTokenResult.Success(d => d.Token = _tokenManager.GenerateToken());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error on generating Aut Token");

                return GetTokenResult.Fail(e);
            }
        }

        /// <summary>
        ///     Versucht ein neues Passwort zu setzen
        /// </summary>
        /// <param name="newPassword">
        ///     Das alte und das neue Passwort das Gesetzt werden soll.
        /// </param>
        /// <returns>
        ///     Das ergebnis des Vorgangs und ein token sovern erfolgreich.
        /// </returns>
        /// <response code="200">Das Ergebnis</response>
        [HttpPost("SetPassword")]
        [ProducesResponseType(typeof(SetPasswordResult), (int) HttpStatusCode.OK)]
        public async Task<SetPasswordResult> Setpassword([FromBody] NewPasswordData newPassword)
        {
            LogContext.PushProperty("Client", HttpContext.Connection.Id);

            try
            {
                if (!await _passwordVault.CheckPassword(newPassword.OldPassword))
                    return SetPasswordResult.Fail("Falsches Passwort angegeben");

                if (!await _passwordVault.SetPassword(newPassword.NewPassword))
                    return SetPasswordResult.Fail("Fehler beim setzen des Passwortes");

                return SetPasswordResult.Success(d => d.Token = _tokenManager.GenerateToken());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error on set Passwort");

                return SetPasswordResult.Fail(e);
            }
        }
    }
}