using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tauron.Application.MgiProjectManager.Server.Areas.Setup.Pages
{
    public class CreateUserModel : PageModel
    {
        private readonly IBaseSettingsManager _manager;

        public CreateUserModel(IBaseSettingsManager manager)
        {
            _manager = manager;
        }

        [BindProperty] public Admin AdminInput { get; set; }

        public IActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            AdminInput = new Admin
            {
                Email = _manager.BaseSettings.UserName,
                Password = _manager.BaseSettings.Password
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            if (!ModelState.IsValid) return Page();

            var set = _manager.BaseSettings;
            set.Password = AdminInput.Password;
            set.UserName = AdminInput.Email;

            return RedirectToPage("/SetFilePath", new {area = "Setup"});
        }

        public sealed class AdminValidator : AbstractValidator<Admin>
        {
            public AdminValidator()
            {
                RuleFor(p => p.Password)
                    .NotEmpty()
                    .Length(6, 100);

                RuleFor(p => p.Email)
                    .NotEmpty()
                    .Length(3, 100)
                    .EmailAddress();
            }
        }

        public sealed class Admin
        {
            [BindProperty] public string Email { get; set; }

            [BindProperty] public string Password { get; set; }
        }
    }
}