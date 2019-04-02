using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public InputModel Input { get; set; }

        public IActionResult OnGet(string code = null)
        {
            if (code == null) return BadRequest(WebResources.ResetPassword_Text_NoCode);

            Input = new InputModel
            {
                Code = code
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null) return RedirectToPage("./ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded) return RedirectToPage("./ResetPasswordConfirmation");

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            [EmailAddress(ErrorMessageResourceName = "Data_EmailAddress_Invalid", ErrorMessageResourceType = typeof(WebResources))]
            public string Email { get; set; }

            [Required(ErrorMessageResourceName = "Data_Required", ErrorMessageResourceType = typeof(WebResources))]
            [StringLength(100, ErrorMessageResourceName = "Data_StringLenght", ErrorMessageResourceType = typeof(WebResources), MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "ResetPassword_Data_ConfirmPassword", ResourceType = typeof(WebResources))]
            [Compare("Password", ErrorMessageResourceName = "ResetPassword_Data_ComparePassword", ErrorMessageResourceType = typeof(WebResources))]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }
    }
}