using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class DeleteRepositoryValidator : ValidatorBase<DeleteRepository, DeleteRepositoryValidator>
    {
        public DeleteRepositoryValidator() 
            => RuleFor(dr => dr.Name).NotEmpty().WithMessage("Kein Name Angegeben");
    }
}