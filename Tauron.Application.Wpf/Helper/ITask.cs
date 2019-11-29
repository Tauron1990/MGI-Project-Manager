using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Helper
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