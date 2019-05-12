using System.Threading.Tasks;
using System.Windows.Input;

namespace IISServiceManager.Core
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object param);
        bool CanExecute();
    }
}