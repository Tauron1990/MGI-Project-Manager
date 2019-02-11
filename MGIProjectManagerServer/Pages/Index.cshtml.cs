using System.Globalization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MGIProjectManagerServer.Pages
{
    public class IndexModel : PageModel
    {
        [UsedImplicitly]
        public void OnGet()
        {

        }

        [UsedImplicitly]
        public IActionResult OnGetSetCulture(string culture)
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureInfo.GetCultureInfo(culture))));
            return Redirect("/Index");
        }
    }
}
