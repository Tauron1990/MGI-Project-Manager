using JetBrains.Annotations;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;

namespace MGIProjectManagerServer.Core.Setup
{
    public interface IBaseSettingsManager
    {
        [NotNull] BaseSettings BaseSettings { get; }

        void Read();
        void Save();
    }
}