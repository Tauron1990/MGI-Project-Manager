using System;
using System.Security.Cryptography;
using System.Text;

namespace Tauron.MgiManager.User.Service.Core
{
    public static class PasswordGenerator
    {
        public static (string hash, string salt) Hash(string password)
        {
            string salt = Guid.NewGuid().ToString();

            using Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 50);

            return (Convert.ToBase64String(hasher.GetBytes(50)), salt);
        }

        public static bool Compare(string password, string salt, string hash)
        {
            using var hasher = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 50);

            return Convert.ToBase64String(hasher.GetBytes(50)) == hash;
        }
    }
}