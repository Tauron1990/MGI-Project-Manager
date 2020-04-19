using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class CheckRequestValidator : ValidatorBase<CheckRequest, CheckRequestValidator>
    {
        public CheckRequestValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Keine Name für die Anwendung Angegeben");
            RuleFor(r => r.Version).Custom(ValidateVersion);
        }

        private void ValidateVersion(string value, CustomContext context)
        {
            if(Version.TryParse(value, out _))
                return;

            context.AddFailure("Version hat ein Falsches Format");
        }
    }
}