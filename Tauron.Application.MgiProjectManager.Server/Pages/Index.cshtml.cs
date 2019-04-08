﻿using System.Globalization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Server.Core.Setup;

namespace Tauron.Application.MgiProjectManager.Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBaseSettingsManager _manager;

        public IndexModel(IBaseSettingsManager manager)
        {
            _manager = manager;
        }

        [UsedImplicitly]
        public ActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated)
                return Page();

            return RedirectToPage("/Start", new {area = "Setup"});
        }

        [UsedImplicitly]
        public IActionResult OnGetSetCulture(string culture)
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureInfo.GetCultureInfo(culture))));
            return Redirect("/Index");
        }
    }
}