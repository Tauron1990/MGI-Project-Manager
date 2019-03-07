using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Hosting;

namespace MGIProjectManagerServer.Core.Setup.Impl
{
    public sealed class BaseSettingsManager : IBaseSettingsManager
    {
        private const string RelativeSettingsPath = "Settings.bin";

        private readonly IHostingEnvironment _hostingEnvironment;
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        public BaseSettings BaseSettings { get; private set; } = new BaseSettings();

        public BaseSettingsManager(IHostingEnvironment hostingEnvironment) 
            => _hostingEnvironment = hostingEnvironment;

        public void Read()
        {
            try
            {
                using (var stream = File.OpenWrite(GetPath()))
                    BaseSettings = (BaseSettings) BinaryFormatter.Deserialize(stream);
            }
            catch
            {
                BaseSettings = new BaseSettings();
            }
        }

        public void Save()
        {
            using (var stream = File.OpenWrite(GetPath()))
                BinaryFormatter.Serialize(stream, BaseSettings);
        }

        private string GetPath() 
            => _hostingEnvironment.WebRootFileProvider.GetFileInfo(RelativeSettingsPath).PhysicalPath;
    }
}
