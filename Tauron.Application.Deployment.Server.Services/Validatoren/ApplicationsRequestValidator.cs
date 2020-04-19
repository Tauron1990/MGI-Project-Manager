using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class ApplicationsRequestValidator : ValidatorBase<ApplicationsRequest, ApplicationsRequestValidator>
    {
        public ApplicationsRequestValidator() 
            => RuleFor(a => a.Name).NotEmpty().Unless(a => !a.All).WithMessage("Kein Repository Name Angegeben");
    }
}