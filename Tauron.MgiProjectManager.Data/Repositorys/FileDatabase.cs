using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Repositorys
{
    [Export(typeof(IFileDatabase))]
    public class FileDatabase : IFileDatabase
    {
        private readonly FilesDbContext _context;

        public FileDatabase(FilesDbContext context)
        {
            _context = context;
        }

        public Task<long> ComputeSize()
        {
            return _context.FileInfos.SumAsync(m => m.Size);
        }

        public async Task<IEnumerable<FileBlobInfoEntity>> GetOldestBySize(long maxSize)
        {
            long currentSize = 0;

            var erg = (await _context.FileInfos
                    .OrderBy(m => m.CreationTime)
                    .ToListAsync())
                .Where(i =>
                {
                    currentSize += i.Size;
                    return currentSize < maxSize;
                });

            return erg;
        }

        public async Task Delete(IEnumerable<FileBlobInfoEntity> filesToDelete)
        {
            foreach (var entity in filesToDelete)
            {
                _context.Blobs.Remove(await _context.Blobs.FindAsync(entity.FileName));
                _context.FileInfos.Remove(entity);
            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddFile(string name, Func<Task<Stream>> binaryData)
        {
            if (await _context.FileInfos.FindAsync(name) != null)
                return false;

            var info = new FileBlobInfoEntity
            {
                FileName = name,
                CreationTime = DateTime.Now
            };
            var blob = new FileBlobEntity
            {
                Key = name
            };

            using var input = await binaryData();
            using var output = new MemoryStream();

            await Compressor.Compressor.CompressFileLzma(input, output);

            blob.Data = output.ToArray();
            info.Size = blob.Data.LongLength;

            await _context.AddRangeAsync(info, blob);

            return true;
        }

        public async Task<byte[]> GetFile(string name)
        {
            var blob = await _context.Blobs.FindAsync(name);
            if (blob == null) return null;

            using var input = new MemoryStream(blob.Data);
            using var output = new MemoryStream();

            await Compressor.Compressor.DecompressFileLzma(input, output);

            return output.ToArray();
        }
    }
}