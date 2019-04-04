using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Data.Api;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Api.Files
{
    [Route("api/[controller]")]
    [Authorize(Policy = RoleNames.Viewer)]
    [ApiController]
    [DisableRequestSizeLimit]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileManager _fileManager;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IHostingEnvironment hostingEnvironment, IFileManager fileManager, ILogger<FilesController> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileManager = fileManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadFilesAsync()
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            List<string> filesToAdd = new List<string>();

            long size = 0;
            var files = Request.Form.Files;

            string filedic = _hostingEnvironment.WebRootPath + @"\uploadedfiles\";
            if (!Directory.Exists(filedic))
                Directory.CreateDirectory(filedic);

            foreach (var file in files)
            {
                var canAdd = await _fileManager.CanAdd(file.FileName);
                if (!canAdd.Ok)
                {
                    errors[file.FileName] = canAdd.Error;
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

            var result = await _fileManager.AddFiles(filesToAdd);

            if (!result.Ok)
                return BadRequest(result.Operation);

            string message = string.Format(WebResources.Api_FilesController_SucessMessage, files.Count, size);

            return new UploadResult(errors, message, result.Operation);
        }
    }
}