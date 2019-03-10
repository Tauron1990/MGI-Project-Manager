using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;

namespace MGIProjectManagerServer.Core.Setup.Impl
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class BaseSettingsManager : IBaseSettingsManager
    {
        private const string RelativeSettingsPath = "Settings.bin";

        private readonly IHostingEnvironment _hostingEnvironment;
        private static readonly System.Runtime.Serialization.Formatters.Binary.BinaryFormatter BinaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        public BaseSettings BaseSettings { get; private set; } = new BaseSettings();

        public BaseSettingsManager(IHostingEnvironment hostingEnvironment) 
            => _hostingEnvironment = hostingEnvironment;

        public void Read()
        {
            try
            {
                using (var stream = File.OpenRead(GetPath()))
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
