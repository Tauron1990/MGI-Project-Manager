using JetBrains.Annotations;

namespace MGIProjectManagerServer.Core.Setup
{
    public interface IBaseSettingsManager
    {
        [NotNull]
        BaseSettings BaseSettings { get; }

        void Read();
        void Save();
    }
}