using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Resources;

namespace Tauron.MgiProjectManager.Model.Api.Validators
{
    [UsedImplicitly]
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage(ContractsRes.Models_Api_RoleViewModel_Name_Empty)
                .Length(2, 200).WithMessage(ContractsRes.Models_Api_RoleViewModel_Name_Lenght);
        }
    }
}