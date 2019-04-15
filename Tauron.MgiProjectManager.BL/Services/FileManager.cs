using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Model;
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
            bool ok = AllowedEndings.Any(name.EndsWith);
            if (ok)
                message = string.Format(BLRes.Api_FilesController_DisallowedExtension, Path.GetFileName(name));

            return Task.FromResult((Ok: ok, message));
        }

        public async Task<(bool Ok, string Operation)> AddFiles(IEnumerable<string> name, string userName)
        {
            try
            {
                var op = new OperationSetup(OperationNames.MultiFileOperation, OperationNames.FileOperationType, name.ToDictionary(Path.GetFileName), 
                    DateTime.Now + TimeSpan.FromDays(3));
                op.OperationContext[OperationMeta.MultiFile.UserName] = userName;
                
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
    }
}