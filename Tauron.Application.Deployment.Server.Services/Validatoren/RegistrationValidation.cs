using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class RegistrationValidation : ValidatorBase<Registration, RegistrationValidation>
    {
        public RegistrationValidation() 
            => RuleFor(r => r.Name).NotEmpty().WithMessage("Keine Name für die Push Nachricht angegeben.");
    }
}