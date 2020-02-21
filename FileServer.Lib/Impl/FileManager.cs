using System;
using System.IO;
using System.Threading.Tasks;
using FileServer.Lib.Abstractions;
using FileServer.Lib.Impl.Operations;

namespace FileServer.Lib.Impl
{
    public sealed class FileManager : IFileManager
    {
        private readonly IFileOperationManager _manager;
        private readonly FileServerConfig _fileServerConfig;
        private readonly IFileAccess _fileAccess;

        public FileManager(IFileOperationManager manager, FileServerConfig fileServerConfig, IFileAccess fileAccess)
        {
            _manager = manager;
            _fileServerConfig = fileServerConfig;
            _fileAccess = fileAccess;
        }

        private async Task<TResult> Protect<TResult>(Func<Task<TResult>> action, Func<string, TResult> onError)
        {
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                return onError($"{FileErrors.Exception}:{e.Message}");
            }
        }

        private string GetFullPath(string name)
            => Path.GetFullPath(name, _fileServerConfig.FilesLocation);

        public Task<bool> ExisFile(string name) 
            => _fileAccess.ExisFile(GetFullPath(name));

        public Task<FileOperation> CreateFile(string name, bool @override)
        {
            return Protect(async () =>
            {
                var fullPath = GetFullPath(name);
                if(await _fileAccess.ExisFile(fullPath) && !@override)
                    return new FileOperation(FileErrors.FileExis, Guid.Empty, false);

                var id = Guid.NewGuid();
                var op = new StreamFileOperation(fullPath, await _fileAccess.OpenWrite(fullPath, @override), _fileAccess.DeleteFile);

                var isOk = _manager.Set(op, id);

                return isOk ? new FileOperation(name, id, true) : new FileOperation(FileErrors.OperationSetFail, Guid.Empty, false);
            }, e => new FileOperation(e, Guid.Empty, false));
        }

        public Task<FileResult> WriteChunk(Guid id, byte[] chunk)
        {
            return Protect(async () =>
            {
                using var op = _manager.Get<StreamFileOperation>(id);
                if(op == null)
                    return new FileResult(true, FileErrors.NoOperation);

                await op.Operation.Stream.WriteAsync(chunk);
                return new FileResult(false, string.Empty);

            }, s => new FileResult(true, s));
        }

        public Task<FileOperation> ReadFile(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ReadResult> ReadChunk(Guid id, int requestLenght)
        {
            throw new NotImplementedException();
        }

        public Task<FileResult> DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        public Task<FileResult> FhinishOperation(Guid id, string hash)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValid(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}