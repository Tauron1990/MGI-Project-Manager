using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly SimpleLoc _simpleLoc;
        private readonly UserManager<IdentityUser> _userManager;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender, SimpleLoc simpleLoc)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _simpleLoc = simpleLoc;
        }

        [BindProperty] public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user)) return RedirectToPage("./ForgotPasswordConfirmation");

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    null,
                    new {code},
                    Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    _simpleLoc["ForgotPassword_ResetEmail_Subject"],
                    _simpleLoc["ForgotPassword_ResetMail_Body", HtmlEncoder.Default.Encode(callbackUrl)]);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            [EmailAddress(ErrorMessageResourceName = "Data_EmailAddress_Invalid", ErrorMessageResourceType = typeof(WebResources))]
            public string Email { get; set; }
        }
    }
}