using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Identity.Resources;

namespace Tauron.MgiProjectManager.Identity.Models.Validators
{
    [UsedImplicitly]
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            //Validation logic here
            Include(new UserBaseViewModelValidator());
            RuleFor(m => m.Roles).Must(arr => arr.Length > 0).WithMessage(IdentRes.Models_Api_UserViewModel_Roles_Lenght);
        }
    }
}