using System;
using System.Linq;

namespace Tauron.Application.SimpleAuth.Core
{
    public sealed class TokenManager : ITokenManager
    {
        public string GenerateToken()
        {
            var time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public bool ValidateToken(string token)
        {
            var data = Convert.FromBase64String(token);
            var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            return when >= DateTime.UtcNow.AddHours(-24);
        }

    }
}