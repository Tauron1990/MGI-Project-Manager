using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class ApplicationRequestValidator : ValidatorBase<ApplicationRequest, ApplicationRequestValidator>
    {
        public ApplicationRequestValidator() 
            => RuleFor(ar => ar.Name).NotEmpty().WithMessage("Name der Anwendung nicht angegeben");
    }
}