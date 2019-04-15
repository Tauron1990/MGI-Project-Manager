using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Core.Setup;
using Tauron.Application.MgiProjectManager.Server.Data.Api;
using Tauron.Application.MgiProjectManager.Server.Data.Core;
using Tauron.MgiProjectManager.BL.Services;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Model.Api;

namespace Tauron.Application.MgiProjectManager.Server.api.Files
{
    [Route("api/[controller]")]
    [Authorize(Policy = RoleNames.Viewer)]
    [ApiController]
    [DisableRequestSizeLimit]
    public class FilesController : ControllerBase
    {
        private readonly IFileManager _fileManager;
        private readonly ILogger<FilesController> _logger;
        private readonly IBaseSettingsManager _baseSettingsManager;
        private readonly IOperationManager _operationManager;

        public FilesController(IFileManager fileManager, ILogger<FilesController> logger, IBaseSettingsManager baseSettingsManager, IOperationManager operationManager)
        {
            _fileManager = fileManager;
            _logger = logger;
            _baseSettingsManager = baseSettingsManager;
            _operationManager = operationManager;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadFiles([FromForm]List<IFormFile> files)//([FromForm]UploadedFile filesContent)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            List<string> filesToAdd = new List<string>();

            long size = 0;
            files = new List<IFormFile>(files ?? (IEnumerable<IFormFile>) Request.Form.Files);

            string filedic = _baseSettingsManager.BaseSettings.FullSaveFilePath;
            if (!Directory.Exists(filedic))
                Directory.CreateDirectory(filedic);

            foreach (var file in files)
            {
                var (ok, error) = await _fileManager.CanAdd(file.FileName);
                if (!ok)
                {
                    errors[file.FileName] = error;
                    continue;
                }

                string filename = filedic + file.FileName;

                try
                {
                    size += file.Length;
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }

                    filesToAdd.Add(filename);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error on Upload File!");
                    errors[file.FileName] = e.Message;
                }
            }

            var result = await _fileManager.AddFiles(filesToAdd, User.Identity.Name);

            if (!result.Ok)
                return BadRequest(result.Operation);

            string message = string.Format(WebResources.Api_FilesController_SucessMessage, files.Count, size);

            return new UploadResult(errors, message, result.Operation);
        }

        [HttpPost]
        [Route("GetUnAssociateFiles")]
        public async Task<ActionResult<UnAssociateFile[]>> GetUnAssociateFiles(string opId)
        {
            var ops = await _operationManager.GetOperations(op =>
            {
                if (op.OperationType != OperationNames.LinkingFileOperation) return false;
                if (string.IsNullOrWhiteSpace(opId)) return true;
                return op.OperationContext[OperationMeta.Linker.StartId] == opId;
            });

            return await ops.ToAsyncEnumerable().Select(op => new UnAssociateFile
            {
                FileName = op.OperationContext[OperationMeta.Linker.FileName],
                OperationId = op.OperationId
            }).ToArray();
        }

        [HttpPost]
        [Route("PostAssociateFile")]
        public async Task PostAssociateFile(AssociateFile file)
        {
            var op = await _operationManager.SearchOperation(file.OperationId);
            op.OperationContext[OperationMeta.Linker.RequestedName] = file.JobNumber;

            await _operationManager.UpdateOperation(op);
            await _operationManager.ExecuteNext(op);
        }

        [HttpPost]
        [Route("StartMultifile")]
        [ProducesResponseType(200, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(string))]
        public async Task<IActionResult> StartMultiFile(string id)
        {
            try
            {
                var op = await _operationManager.SearchOperation(id);
                if (op == null)
                    return BadRequest(WebResources.Api_FilesController_NoOperationFound);
                if (op.OperationType != OperationNames.MultiFileOperation)
                    return BadRequest(WebResources.Api_FilesController_NotCompatible);
                var resultOps = await _operationManager.ExecuteNext(op);

                foreach (var operation in resultOps)
                    await _operationManager.AddOperation(operation);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on start Multifile Operation: {id}");

                return BadRequest(e.Message);
            }
        }
    }
}