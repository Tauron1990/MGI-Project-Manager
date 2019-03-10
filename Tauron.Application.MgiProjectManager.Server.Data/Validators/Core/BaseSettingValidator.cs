using System.IO;
using System.Net;
using FluentValidation;
using JetBrains.Annotations;
using Tauron.Application.MgiProjectManager.Resources.Web;
using Tauron.Application.MgiProjectManager.Server.Data.Core.Setup;

namespace Tauron.Application.MgiProjectManager.Server.Data.Validators.Core
{
    [UsedImplicitly]
    public sealed class BaseSettingValidator : AbstractValidator<BaseSettings>
    {
        public BaseSettingValidator()
        {
            RuleFor(p => p.Password)
                .NotEmpty()
                .Length(6, 100);

            RuleFor(p => p.UserName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(p => p.SaveFilePath)
                .NotEmpty()
                .WithName(WebResources.BaseSettings_FilePath_Name)
                .Custom((value, context) =>
                {
                    var invalid = Path.GetInvalidPathChars();

                    if (value.IndexOfAny(invalid) != -1)
                    {
                        context.AddFailure(context.PropertyName, WebResources.BaseSettings_FilePath_InvalidChars);
                    }
                });
        }
    }
}