using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    public interface IFileDatabase
    {
        Task<long> ComputeSize();
        Task<IEnumerable<FileBlobInfoEntity>> GetOldestBySize(long maxSize);
        Task Delete(IEnumerable<FileBlobInfoEntity> filesToDelete);
        Task SaveChanges();

        Task<bool> AddFile(string name, Func<Task<Stream>> binaryData);

        Task<byte[]> GetFile(string name);
    }
}