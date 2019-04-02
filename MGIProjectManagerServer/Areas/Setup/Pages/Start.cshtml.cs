using MGIProjectManagerServer.Core.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;

namespace MGIProjectManagerServer.Areas.Setup.Pages
{
    [BindProperties]
    public class StartModel : PageModel
    {
        public StartModel(IBaseSettingsManager manager)
        {
            BaseSettings = manager.BaseSettings;
        }

        public BaseSettings BaseSettings { get; }

        public IActionResult OnGet()
        {
            if (BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            return Page();
        }

        public IActionResult OnGetNext()
        {
            return RedirectToPage("/CreateUser", new {area = "Setup"});
        }
    }
}