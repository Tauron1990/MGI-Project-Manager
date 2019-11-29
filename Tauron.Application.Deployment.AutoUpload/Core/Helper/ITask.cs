using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload.Core.Helper
{
    public interface ITask
    {
        Task ExecuteAsync();

        void ExecuteSync();

        bool Synchronize { get; }

        [NotNull]
        Task Task { get; }
    }
}