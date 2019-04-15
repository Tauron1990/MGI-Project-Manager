using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface IFileRepository
    {
        Task AddFile(FileEntity entity);

        Task<FileEntity[]> GetUnRequetedFiles();

        Task DeleteFile(int id);
    }
}