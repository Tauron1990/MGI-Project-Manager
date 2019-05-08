using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface IFileRepository
    {
        Task AddFile(FileEntity entity);

        Task<FileEntity[]> GetUnRequestedFiles();

        Task DeleteFile(int id);
    }
}