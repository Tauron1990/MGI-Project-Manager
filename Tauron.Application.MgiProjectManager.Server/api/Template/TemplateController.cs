using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Core;
using Tauron.Application.MgiProjectManager.Server.Data.Api;

namespace Tauron.Application.MgiProjectManager.Server.api.Template
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(IRazorPartialToStringRenderer renderer, ILogger<TemplateController> logger)
        {
            _renderer = renderer;
            _logger = logger;
        }

        [HttpPost]
        [Route("AdminGridError")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesErrorResponseType(typeof(string))]
        [Consumes("text/plain")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> GetErrorContent(string jObject)
        {
            try
            {
                return await _renderer.RenderPartialToStringAsync(base.PartialView("Templates/AdminDialogError", JObject.Parse(jObject)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Redering AdminDialogError");

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("FileUploadError")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesErrorResponseType(typeof(string))]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> GetUploadErrorContent(UploadResult result)
        {
            try
            {
                return await _renderer.RenderPartialToStringAsync(base.PartialView("Templates/UploadDialogError", result));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Redering AdminDialogError");

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("LinkingFileTemplate")]
        [ProducesErrorResponseType(typeof(string))]
        public async Task<ActionResult<FileToNameTemplate>> GetLinkingFileTemplate(UnAssociateFile file)
        {
            try
            {
                var template = await _renderer.RenderPartialToStringAsync(base.PartialView("Templates/FileToNameElement", file));

                return new FileToNameTemplate(file.OperationId, template, WebResources.Template_FileToName_Submit);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Redering AdminDialogError");

                return BadRequest(e.Message);
            }
        }
    }
}