using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Model;
using Tauron.MgiProjectManager.Model.Api;
using Tauron.MgiProjectManager.Resources;

namespace Tauron.MgiProjectManager.BL.Services
{
    [Export(typeof(IFileManager))]
    public class FileManager : IFileManager
    {
        private static readonly string[] AllowedEndings = { ".pdf", ".tif", ".tiff", ".zip" };

        private readonly IOperationManager _operationManager;
        private readonly ILogger<FileManager> _logger;
        private readonly IFileDatabase _fileDatabase;

        public FileManager(IOperationManager operationManager, ILogger<FileManager> logger, IFileDatabase fileDatabase)
        {
            _operationManager = operationManager;
            _logger = logger;
            _fileDatabase = fileDatabase;
        }

        public Task<(bool Ok, string Error)> CanAdd(string name)
        {
            string message = string.Empty;
            bool ok = AllowedEndings.Any(name.EndsWith) && name.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries).Length == 2;
            if (ok)
                message = string.Format(BLRes.Api_FilesController_DisallowedExtension, Path.GetFileName(name));

            return Task.FromResult((Ok: ok, message));
        }

        public async Task<(bool Ok, string Operation)> AddFiles(List<(string Name, Stream Stream)> files, string userName, Action<string> postError)
        {
            try
            {
                var op = new OperationSetup(OperationNames.MultiFileOperation, OperationNames.FileOperationType, new Dictionary<string, string>(), 
                    DateTime.Now + TimeSpan.FromDays(3));
                op.OperationContext[OperationMeta.MultiFile.UserName] = userName;

                foreach (var (name, stream) in files)
                {
                    // ReSharper disable once ConvertToUsingDeclaration
                    using (stream)
                    {
                        if (await _fileDatabase.AddFile(name, () => Task.FromResult(stream)))
                            op.OperationContext[name] = name;
                        else postError(name);
                    }
                }

                await _fileDatabase.SaveChanges();

                return (true, await _operationManager.AddOperation(op));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding Files");

                return (false, e.Message);
            }
        }

        public Task DeleteFile(string path)
        {
            _logger.LogInformation($"File Deleted: {path}");

            File.Delete(path);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<UnAssociateFile>> GetUnAssociateFile(string opId)
        {
            var ops = await _operationManager.GetOperations(op =>
            {
                if (op.OperationType != OperationNames.LinkingFileOperation) return false;
                if (string.IsNullOrWhiteSpace(opId)) return true;
                return op.Context[OperationMeta.Linker.StartId] == opId;
            });

            List<UnAssociateFile> files = new List<UnAssociateFile>();

            foreach (var op in ops)
            {
                var contex = await _operationManager.GetContext(op);
                string name = contex[OperationMeta.Linker.FileName];

                files.Add(new UnAssociateFile
                {
                    FileName = name,
                    OperationId = op
                });
            }

            return files;
        }

        public async Task PostAssociateFile(AssociateFile file)
        {
            await _operationManager.UpdateOperation(file.OperationId, dictionary => dictionary[OperationMeta.Linker.RequestedName] = file.JobNumber);
            await _operationManager.ExecuteNext(file.OperationId);
        }

        public async Task<(bool Ok, string Error)> StartMultiFile(string id)
        {
            var op = await _operationManager.SearchOperation(id);
            if (op == null)
                return (false, BLRes.Api_FilesController_NoOperationFound);
            if (op[OperationMeta.OperationName] != OperationNames.MultiFileOperation)
                return (false, BLRes.Api_FilesController_NotCompatible);
            await _operationManager.ExecuteNext(id);

            return (true, String.Empty);
        }
    }
}