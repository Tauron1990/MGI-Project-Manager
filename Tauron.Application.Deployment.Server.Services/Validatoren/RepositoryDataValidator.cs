using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class RepositoryDataValidator : ValidatorBase<RegisterRepositoryData, RepositoryDataValidator>
    {
        public RepositoryDataValidator()
        {
            RuleFor(rrd => rrd.Source).NotEmpty().WithMessage("Die Quelle des Repositorys Darf nicht Leer sein");
            RuleFor(r => r.Provider).NotEmpty().WithMessage("Der provider muss angegeben werden");
            RuleFor(r => r.Name).NotEmpty().WithMessage("Der Name muss Angegeben werden");
        }
    }
}