using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MGIProjectManagerServer.Areas.File.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public IndexModel(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnGet()
        {
        }

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