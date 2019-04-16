using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Resources;

namespace Tauron.MgiProjectManager.Model.Api.Validators
{
    [UsedImplicitly]
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            //Validation logic here
            Include(new UserBaseViewModelValidator());
            RuleFor(m => m.Roles).Must(arr => arr.Length > 0).WithMessage(ContractsRes.Models_Api_UserViewModel_Roles_Lenght);
        }
    }
}