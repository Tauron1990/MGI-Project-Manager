using System;
using System.Threading.Tasks;

namespace IISServiceManager.Contratcs
{
    public interface ILog : IDisposable
    {
        bool AutoClose { get; set; }

        void WriteLine(string content);

        Task EnterOperation();

        Task ExitOperation();
    }
}