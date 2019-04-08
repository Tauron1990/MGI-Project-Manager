using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace Tauron.Application.MgiProjectManager.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SimpleLoc _simpleLoc;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, SimpleLoc simpleLoc)
        {
            _signInManager = signInManager;
            _logger = logger;
            _simpleLoc = simpleLoc;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid) return Page();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor) return RedirectToPage("./LoginWith2fa", new {ReturnUrl = returnUrl, Input.RememberMe});
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, _simpleLoc["Login_Text_LoginError"]);

            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            //[EmailAddress(ErrorMessageResourceName = "Data_EmailAddress_Invalid", ErrorMessageResourceType = typeof(WebResources))]
            public string Email { get; set; }

            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Data_Display_RememberMe", ResourceType = typeof(WebResources))]
            public bool RememberMe { get; set; }
        }
    }
}