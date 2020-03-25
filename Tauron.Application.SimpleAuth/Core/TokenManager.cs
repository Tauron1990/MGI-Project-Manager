using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace Tauron.Application.SimpleAuth.Core
{
    public sealed class TokenManager : ITokenManager
    {
        private readonly ISystemClock _clock;

        public TokenManager(ISystemClock clock)
        {
            _clock = clock;
        }

        public string GenerateToken()
        {
            var time = BitConverter.GetBytes(_clock.UtcNow.Date.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public bool ValidateToken(string token)
        {
            var data = Convert.FromBase64String(token);
            var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when >= _clock.UtcNow.AddHours(-24);
        }

    }
}