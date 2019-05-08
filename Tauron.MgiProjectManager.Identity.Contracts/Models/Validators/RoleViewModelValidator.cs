using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Identity.Resources;

namespace Tauron.MgiProjectManager.Identity.Models.Validators
{
    [UsedImplicitly]
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage(IdentRes.Models_Api_RoleViewModel_Name_Empty)
                .Length(2, 200).WithMessage(IdentRes.Models_Api_RoleViewModel_Name_Lenght);
        }
    }
}