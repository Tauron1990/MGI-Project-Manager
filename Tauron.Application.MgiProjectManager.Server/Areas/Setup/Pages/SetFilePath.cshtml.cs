using System.IO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Core.Setup;

namespace Tauron.Application.MgiProjectManager.Server.Areas.Setup.Pages
{
    public class SetFilePathModel : PageModel
    {
        private readonly IBaseSettingsManager _manager;

        public SetFilePathModel(IBaseSettingsManager manager)
        {
            _manager = manager;
        }

        [BindProperty] public FilePathModel Model { get; set; }

        public IActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            Model = new FilePathModel
            {
                Path = _manager.BaseSettings.SaveFilePath
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new {area = ""});

            if (!ModelState.IsValid) return Page();

            _manager.BaseSettings.SaveFilePath = Model.Path;

            return RedirectToPage("/Finish");
        }

        public class FilePathModelValidator : AbstractValidator<FilePathModel>
        {
            public FilePathModelValidator()
            {
                RuleFor(p => p.Path)
                    .NotEmpty()
                    .WithName(WebResources.BaseSettings_FilePath_Name)
                    .Custom((value, context) =>
                    {
                        var invalid = Path.GetInvalidPathChars();

                        if (value == null) return;

                        if (value.IndexOfAny(invalid) != -1)
                            context.AddFailure(WebResources.BaseSettings_FilePath_InvalidChars);
                    });
            }
        }

        public class FilePathModel
        {
            [BindProperty] public string Path { get; set; }
        }
    }
}