using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FileServer.Lib
{
    [PublicAPI]
    public interface IFileManager
    {
        Task<bool> ExisFile(string name);

        Task<FileOperation> CreateFile(string name, bool @override);

        Task<FileResult> WriteChunk(Guid id, byte[] chunk);

        Task<FileOperation> ReadFile(string name);

        Task<ReadResult> ReadChunk(Guid id, int requestLenght);

        Task<FileResult> DeleteFile(string name);

        Task<FileResult> FhinishOperation(Guid id, string hash);

        Task<bool> IsValid(Guid id);
    }
}