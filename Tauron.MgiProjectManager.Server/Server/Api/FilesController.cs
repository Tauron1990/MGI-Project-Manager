using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.BL.Services;
using Tauron.MgiProjectManager.Model.Api;
using Tauron.MgiProjectManager.Resources;
using Tauron.MgiProjectManager.Server.Authorization;

namespace Tauron.MgiProjectManager.Server.Api
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.UploadFilesPolicy)]
    [ApiController]
    [DisableRequestSizeLimit]
    public class FilesController : ControllerBase
    {
        private readonly IFileManager _fileManager;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IFileManager fileManager, ILogger<FilesController> logger)
        {
            _fileManager = fileManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadFiles([FromForm]List<IFormFile> files)//([FromForm]UploadedFile filesContent)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            List<(string Name, Stream Stream)> filesToAdd = new List<(string Name, Stream Stream)>();

            long size = 0;
            files = new List<IFormFile>(files ?? (IEnumerable<IFormFile>) Request.Form.Files);
            

            foreach (var file in files)
            {
                var realFileName = Path.GetFileName(file.FileName);
                var (ok, error) = await _fileManager.CanAdd(realFileName);
                if (!ok)
                {
                    errors[file.FileName] = error;
                    continue;
                }

                try
                {
                    filesToAdd.Add((realFileName, file.OpenReadStream()));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error on Upload File!");
                    errors[file.FileName] = e.Message;
                }
            }

            var result = await _fileManager.AddFiles(filesToAdd, User.Identity.Name, s => errors[s] = string.Format(BLRes.Api_FilesController_FileNotAddable, s));

            if (!result.Ok)
                return BadRequest(result.Operation);

            string message = string.Format(BLRes.Api_FilesController_SucessMessage, filesToAdd.Count, size);

            return new UploadResult(errors, message, result.Operation);
        }

        [HttpPost]
        [Route("GetUnAssociateFiles")]
        public async Task<ActionResult<UnAssociateFile[]>> GetUnAssociateFiles(string opId) 
            => (await _fileManager.GetUnAssociateFile(opId)).ToArray();

        [HttpPost]
        [Route("PostAssociateFile")]
        public Task PostAssociateFile(AssociateFile file) 
            => _fileManager.PostAssociateFile(file);

        [HttpPost]
        [Route("StartMultifile")]
        [ProducesResponseType(200, Type = typeof(void))]
        [ProducesErrorResponseType(typeof(string))]
        public async Task<IActionResult> StartMultiFile(string id)
        {
            try
            {
                var result = await _fileManager.StartMultiFile(id);
                return result.Ok ? (IActionResult) Ok() : BadRequest(result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on start Multifile Operation: {id}");

                return BadRequest(e.Message);
            }
        }
    }
}