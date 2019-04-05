using System;
using System.Threading.Tasks;
using FluentValidation;
using MGIProjectManagerServer.Core;
using MGIProjectManagerServer.Core.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;
using Tauron.Application.MgiProjectManager.Server.Data.Validators.Core;

namespace MGIProjectManagerServer.Areas.Setup.Pages
{
    public class FinishModel : PageModel
    {
        private readonly IBaseSettingsManager _manager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public FinishModel(IBaseSettingsManager manager, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _manager = manager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty] public BaseSettings BaseSettings { get; set; }

        public IActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});
            BaseSettings = _manager.BaseSettings;

            return Page();
        }

        public async Task<IActionResult> OnGetNext()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            var validator = new BaseSettingValidator();
            validator.ValidateAndThrow(_manager.BaseSettings);
            _manager.BaseSettings.IsConfigurated = true;

            _manager.BaseSettings.FullSaveFilePath.CreateDirectoryIfNotExis();


            foreach (var roleName in RoleNames.GetAllRoles())
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var user = new IdentityUser(_manager.BaseSettings.UserName);
            var userResult = await _userManager.CreateAsync(user, _manager.BaseSettings.Password);

            if (userResult.Succeeded)
                await _userManager.AddToRoleAsync(user, RoleNames.Admin);
            else
                throw new InvalidOperationException("Admin creation faild.");

            _manager.Save();
            return RedirectToPage("/Index", new {area = ""});
        }
    }
}