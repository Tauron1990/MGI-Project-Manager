using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using MGIProjectManagerServer.Core.Setup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Data.Api;

namespace MGIProjectManagerServer.Api.Files
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

        public FilesController(IFileManager fileManager, ILogger<FilesController> logger, IBaseSettingsManager baseSettingsManager)
        {
            _fileManager = fileManager;
            _logger = logger;
            _baseSettingsManager = baseSettingsManager;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadFilesAsync([FromForm]UploadedFile filesContent)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            List<string> filesToAdd = new List<string>();

            long size = 0;
            List<IFormFile> files = new List<IFormFile>(filesContent.Files ?? (IEnumerable<IFormFile>) Request.Form.Files);

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
    }
}