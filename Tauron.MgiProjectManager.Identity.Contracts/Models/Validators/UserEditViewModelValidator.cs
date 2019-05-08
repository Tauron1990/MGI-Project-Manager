using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Identity.Resources;

namespace Tauron.MgiProjectManager.Identity.Models.Validators
{
    [UsedImplicitly]
    public class UserEditViewModelValidator : AbstractValidator<UserEditViewModel>
    {
        public UserEditViewModelValidator()
        {
            Include(new UserBaseViewModelValidator());

            RuleFor(m => m.NewPassword).MinimumLength(6).WithMessage(IdentRes.Models_Api_UserEditViewModel_NewPassword_Lenght);
            RuleFor(m => m.Roles).Must(arr => arr.Length > 0).WithMessage(IdentRes.Models_Api_UserViewModel_Roles_Lenght);
        }
    }
}