using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace Tauron.Application.MgiProjectManager.BL.Impl
{
    public class FileManager : IFileManager
    {
        private static readonly string[] AllowedEndings = { ".pdf", ".tif", ".tiff", ".zip" };

        private readonly IOperationManager _operationManager;
        private readonly ILogger<FileManager> _logger;

        public FileManager(IOperationManager operationManager, ILogger<FileManager> logger)
        {
            _operationManager = operationManager;
            _logger = logger;
        }

        public Task<(bool Ok, string Error)> CanAdd(string name)
        {
            string message = string.Empty;
            bool ok = AllowedEndings.Any(name.EndsWith);
            if (ok)
                message = string.Format(WebResources.Api_FilesController_DisallowedExtension, Path.GetFileName(name));

            return Task.FromResult((Ok: ok, message));
        }

        public async Task<(bool Ok, string Operation)> AddFiles(IEnumerable<string> name)
        {
            try
            {
                string id = Guid.NewGuid().ToString("D");
                var op = new Operation(id, OperationNames.MultiFileOperation, OperationNames.FileOperationType, name.ToDictionary(Path.GetFileName), 
                    DateTime.Now + TimeSpan.FromDays(3));
                await _operationManager.AddOperation(op);

                return (true, id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding Files");

                return (false, e.Message);
            }
        }
    }
}