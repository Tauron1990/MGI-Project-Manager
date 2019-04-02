using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWithRecoveryCodeModel : PageModel
    {
        private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SimpleLoc _simpleLoc;

        public LoginWithRecoveryCodeModel(SignInManager<IdentityUser> signInManager, ILogger<LoginWithRecoveryCodeModel> logger, SimpleLoc simpleLoc)
        {
            _signInManager = signInManager;
            _logger = logger;
            _simpleLoc = simpleLoc;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) throw new InvalidOperationException("Unable to load two-factor authentication user.");

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) throw new InvalidOperationException("Unable to load two-factor authentication user.");

            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }

            _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
            ModelState.AddModelError(string.Empty, _simpleLoc["LoginWithRecuveryCode_InvalidCode"]);
            return Page();
        }

        public class InputModel
        {
            [BindProperty]
            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            [DataType(DataType.Text)]
            [Display(Name = "Data_Display_RecoveryCode", ResourceType = typeof(WebResources))]
            public string RecoveryCode { get; set; }
        }
    }
}