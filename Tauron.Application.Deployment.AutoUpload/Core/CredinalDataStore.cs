using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    [ServiceDescriptor]
    [UsedImplicitly]
    public sealed class CredinalDataStore
    {
        private readonly string _targetPath;

        public CredinalDataStore()
        {
            _targetPath = Path.Combine(Settings.SettingsDic, "Account");

            if (!Directory.Exists(_targetPath))
                Directory.CreateDirectory(_targetPath);
        }

        public void Delete(string name)
        {
            var fileName = Path.Combine(_targetPath, name + ".acc");

            try
            {
                if (!File.Exists(fileName))
                    return;

                File.Delete(fileName);
            }
            catch(SystemException)
            { }
        }

        public string Get(string name)
        {
            var fileName = Path.Combine(_targetPath, name + ".acc");
            if (!File.Exists(fileName))
                return string.Empty;
            try
            {
                using var reader = new BinaryReader(File.OpenRead(fileName));
                var creds = new CredinalData(reader);

                return Encoding.UTF8.GetString(ProtectedData.Unprotect(creds.Data1, creds.Salt, DataProtectionScope.CurrentUser));
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Set(string? name, string data)
        {
            if(string.IsNullOrWhiteSpace(name)) return;

            var fileName = Path.Combine(_targetPath, name + ".acc");

            try
            {
                using var rng = new RNGCryptoServiceProvider();
                var salt = new byte[128];

                rng.GetBytes(salt);

                var encrypt = ProtectedData.Protect(Encoding.UTF8.GetBytes(data), salt, DataProtectionScope.CurrentUser);

                var creds = new CredinalData(salt, encrypt);

                using var writer = new BinaryWriter(new FileStream(fileName, FileMode.Create));

                creds.Write(writer);
            }
            catch
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                }
                catch (SystemException)
                {
                    //Can Be Ignored
                }
            }
        }

        private class CredinalData
        {
            public CredinalData(byte[] salt, byte[] data1)
            {
                Salt = salt;
                Data1 = data1;
            }

            public CredinalData(BinaryReader reader)
            {
                Salt = reader.ReadBytes(reader.ReadInt32());
                Data1 = reader.ReadBytes(reader.ReadInt32());
            }

            public byte[] Salt { get; }

            public byte[] Data1 { get; }

            public void Write(BinaryWriter writer)
            {
                writer.Write(Salt.Length);
                writer.Write(Salt);
                writer.Write(Data1.Length);
                writer.Write(Data1);
            }
        }
    }
}