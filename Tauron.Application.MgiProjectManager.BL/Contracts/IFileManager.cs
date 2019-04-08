using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.BL.Contracts
{
    public interface IFileManager
    {
        Task<(bool Ok, string Error)> CanAdd(string name);

        Task<(bool Ok, string Operation)> AddFiles(IEnumerable<string> name, string userName);

        Task DeleteFile(string path);
    }
}