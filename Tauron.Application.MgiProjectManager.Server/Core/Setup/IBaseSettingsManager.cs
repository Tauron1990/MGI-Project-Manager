using JetBrains.Annotations;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;

namespace Tauron.Application.MgiProjectManager.Server.Core.Setup
{
    public interface IBaseSettingsManager
    {
        [NotNull] BaseSettings BaseSettings { get; }

        void Read();
        void Save();
    }
}