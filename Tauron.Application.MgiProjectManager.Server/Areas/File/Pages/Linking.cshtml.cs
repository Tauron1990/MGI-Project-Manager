using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tauron.Application.MgiProjectManager.Server.Areas.File.Pages
{
    public class LinkingModel : PageModel
    {
        public string OperationId { get; set; }

        public void OnGet(string id)
        {
            OperationId = id ?? string.Empty;
        }
    }
}