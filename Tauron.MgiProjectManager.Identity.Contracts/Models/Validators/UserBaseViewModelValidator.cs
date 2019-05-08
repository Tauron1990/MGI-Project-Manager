using System.Security.Principal;
using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Identity.Resources;

namespace Tauron.MgiProjectManager.Identity.Models.Validators
{
    [UsedImplicitly]
    public class UserBaseViewModelValidator : AbstractValidator<UserBaseViewModel>
    {
        public UserBaseViewModelValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage(IdentRes.Models_Api_UserViewModel_UserName_Empty)
                .Length(2, 200).WithMessage(IdentRes.Models_Api_UserViewModelBase_UserName_Lenght);

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage(IdentRes.Models_Api_UserViewModelBase_Email_Empty)
                .MaximumLength(200).WithMessage(IdentRes.Models_Api_UserViewModelBase_Email_Lenght)
                .EmailAddress().WithMessage(IdentRes.Models_Api_UserViewModelBase_Email_Invalid);
        }
    }
}