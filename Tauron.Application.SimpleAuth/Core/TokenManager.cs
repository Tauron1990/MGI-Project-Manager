using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Tauron.Application.SimpleAuth.Core
{
    public sealed class TokenManager : ITokenManager, IDisposable
    {
        private static readonly byte[] Salt = Initialize();

        private static byte[] Initialize()
        {
            RandomNumberGenerator gen = new RNGCryptoServiceProvider();
            var array = new byte[256];

            gen.GetBytes(array);
            return array;
        }

        private readonly ISystemClock _clock;
        private readonly IDisposable _subscription;
        private SimpleAuthenticationOptions _options;

        public TokenManager(ISystemClock clock, IOptionsMonitor<SimpleAuthenticationOptions> options)
        {
            _clock = clock;
            _options = options.CurrentValue;
            _subscription = options.OnChange(o => Interlocked.Exchange(ref _options, o));
        }

        public string GenerateToken()
        {
            using var mem = new MemoryStream(350);
            using var writer = new BinaryWriter(mem, Encoding.UTF8);

            writer.Write(_clock.UtcNow.UtcDateTime.ToBinary());
            writer.Write(_options.Realm);
            writer.Write(Salt);

            return Convert.ToBase64String(mem.ToArray());
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var data = Convert.FromBase64String(token);
                using var mem = new MemoryStream(data);
                using var reader = new BinaryReader(mem);

                var when = DateTime.FromBinary(reader.ReadInt64()) + _options.TokenTimeout;
                var realm = reader.ReadString();
                var actualDate = _clock.UtcNow;
                return when > actualDate  && _options.Realm == realm;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose() 
            => _subscription.Dispose();
    }
}