using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.Server.Data.Repository
{
    public interface IFileRepository
    {
        Task AddFile(FileEntity entity);

        Task<FileEntity[]> GetUnRequetedFiles();

        Task DeleteFile(int id);
    }
}