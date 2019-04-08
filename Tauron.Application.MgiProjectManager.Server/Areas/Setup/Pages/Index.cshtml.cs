using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Server.Core.Setup;

namespace Tauron.Application.MgiProjectManager.Server.Areas.Setup.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBaseSettingsManager _manager;

        public IndexModel(IBaseSettingsManager manager)
        {
            _manager = manager;
        }

        public IActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            return Page();
        }
    }
}