using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MGIProjectManagerServer.Api.Files
{
    [Route("api/[controller]")]
    [Authorize(Policy = RoleNames.Viewer)]
    [ApiController]
    [DisableRequestSizeLimit]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FilesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public IActionResult UploadFiles()
        {
            long size = 0;
            var files = Request.Form.Files;

            string filedic = _hostingEnvironment.WebRootPath + @"\uploadedfiles\";
            if (!Directory.Exists(filedic))
                Directory.CreateDirectory(filedic);

            foreach (var file in files)
            {
                string filename = filedic + file.FileName;
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }

            string message = $"{files.Count} file(s) / {size} bytes uploaded successfully!";
            return new JsonResult(message);
        }
    }
}