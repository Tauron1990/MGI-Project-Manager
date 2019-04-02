using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SimpleLoc _simpleLoc;
        private readonly UserManager<IdentityUser> _userManager;

        public DeletePersonalDataModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            SimpleLoc simpleLoc)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _simpleLoc = simpleLoc;
        }

        [BindProperty] public InputModel Input { get; set; }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(_simpleLoc["Account_DeletePersonalData_UserNotFound", _userManager.GetUserId(User)]);

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, WebResources.Account_DeletePersonalData_WrongPassword);
                    return Page();
                }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded) throw new InvalidOperationException(string.Format(WebResources.Account_DeletePersonalData_UnexpectError, userId));

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}