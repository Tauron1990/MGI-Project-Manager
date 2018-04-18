using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NLog;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.ApplicationServer.Data.PrivateEntitys;
using Tauron.Application.ProjectManager.Resources;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    internal static class UserManager
    {
        private static readonly Dictionary<string, UserEntity> UserEntities = new Dictionary<string, UserEntity>();
        private static readonly Logger                         Logger       = LogManager.GetCurrentClassLogger();

        public static void AddInitial(DatabaseImpl db)
        {
            if (db.Users.Find("admin") == null)
            {
                var entity = new UserEntity {Id = "admin", Password = string.Empty, Salt = string.Empty};
                db.Users.Add(entity);
                UserEntities[entity.Id] = entity;
            }

            foreach (var userEntity in db.Users)
            {
                UserEntities[userEntity.Id] = userEntity;
            }
        }

        public static bool HasNoPassword(string userName)
        {
            return UserEntities.TryGetValue(userName, out var ent) && string.IsNullOrWhiteSpace(ent.Password);
        }

        public static bool Validate(string userName, string password, out string reason)
        {
            UserEntities.TryGetValue(userName, out var entity);
            Logger.Info($"Validate User: {userName}");
            return Validate(entity, password, out reason);
        }

        private static bool Validate(UserEntity entity, string password, out string reason)
        {
            if (entity == null)
            {
                reason = ServiceMessages.UserManager_Reason_NoUser;
                return false;
            }

            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(entity.Password))
            {
                reason = "Ok";
                return true;
            }

            var pass = GenerateHash(password, entity.Salt);

            if (entity.Password != pass)
            {
                reason = ServiceMessages.UserManager_Reason_PasswordMismatch;
                return false;
            }

            reason = "Ok";
            return true;
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword, out string reason)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                reason = ServiceMessages.UserManager_Reason_NoPassword;
                return false;
            }

            using (var db = new DatabaseImpl())
            {
                var entity = db.Users.Find(userName);
                if (!Validate(entity, oldPassword, out reason)) return false;

                CreateHash(entity, newPassword);
                UserEntities[entity.Id] = entity;

                Logger.Log(LogLevel.Trace, $"{userName} Password Changed - {DateTime.Today}");
                return true;
            }
        }

        public static bool CreateUser(string userName, string password, out string reason)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                reason = ServiceMessages.UserManager_Reason_NoPassword;
                return false;
            }

            using (var db = new DatabaseImpl())
            {
                if (db.Users.Find(userName) != null)
                {
                    reason = ServiceMessages.UserManager_Reason_UserExis;
                    return false;
                }

                var entity = new UserEntity {Id = userName};
                CreateHash(entity, password);

                db.Users.Add(entity);
                db.SaveChanges();

                UserEntities[entity.Id] = entity;
            }

            reason = "Ok";
            Logger.Log(LogLevel.Trace, $"{userName} User Created - {DateTime.Today}");
            return true;
        }

        public static bool DeleteUser(string userName, out string reason)
        {
            using (var db = new DatabaseImpl())
            {
                var entity = db.Users.Find(userName);
                if (entity == null)
                {
                    reason = ServiceMessages.UserManager_Reason_NoUser;
                    return false;
                }

                db.Remove(entity);
                db.SaveChanges();
                UserEntities.Remove(entity.Id);
            }

            reason = "Ok";
            Logger.Log(LogLevel.Trace, $"{userName} User Deleted - {DateTime.Today}");
            return true;
        }

        private static void CreateHash(UserEntity entity, string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt          = GenerateSalt(20);
            var hash          = GenerateHash(passwordBytes, salt, 20, 50);

            entity.Password = Convert.ToBase64String(hash);
            entity.Salt     = Convert.ToBase64String(salt);
        }

        private static string GenerateHash(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes     = Convert.FromBase64String(salt);

            return Convert.ToBase64String(GenerateHash(passwordBytes, saltBytes, 20, 50));
        }

        private static byte[] GenerateSalt(int length)
        {
            var bytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);

            return bytes;
        }

        private static byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int length)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                return deriveBytes.GetBytes(length);
        }
    }
}