using System.IO;
using FluentValidation;
using MGIProjectManagerServer.Core.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tauron.Application.MgiProjectManager.Resources.Web;

namespace MGIProjectManagerServer.Areas.Setup.Pages
{
    public class SetFilePathModel : PageModel
    {
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

                        if(value == null) return;

                        if (value.IndexOfAny(invalid) != -1)
                            context.AddFailure(WebResources.BaseSettings_FilePath_InvalidChars);
                    });
            }
        }

        public class FilePathModel
        {
            [BindProperty]
            public string Path { get; set; }
        }

        private readonly IBaseSettingsManager _manager;

        [BindProperty]
        public FilePathModel Model { get; set; }

        public SetFilePathModel(IBaseSettingsManager manager) => _manager = manager;

        public IActionResult OnGet()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new { area = "" });

            Model = new FilePathModel
            {
                Path = _manager.BaseSettings.SaveFilePath
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (_manager.BaseSettings.IsConfigurated) return RedirectToPage("/Index", new { area = "" });

            if (!ModelState.IsValid) return Page();

            _manager.BaseSettings.SaveFilePath = Model.Path;

            return RedirectToPage("/Finish");
        }
    }
}