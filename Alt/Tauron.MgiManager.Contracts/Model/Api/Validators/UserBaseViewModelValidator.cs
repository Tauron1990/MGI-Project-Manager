﻿using FluentValidation;
using JetBrains.Annotations;
using Tauron.MgiProjectManager.Resources;

namespace Tauron.MgiProjectManager.Model.Api.Validators
{
    [UsedImplicitly]
    public class UserBaseViewModelValidator : AbstractValidator<UserBaseViewModel>
    {
        public UserBaseViewModelValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage(ContractsRes.Models_Api_UserViewModel_UserName_Empty)
                .Length(2, 200).WithMessage(ContractsRes.Models_Api_UserViewModelBase_UserName_Lenght);

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage(ContractsRes.Models_Api_UserViewModelBase_Email_Empty)
                .MaximumLength(200).WithMessage(ContractsRes.Models_Api_UserViewModelBase_Email_Lenght)
                .EmailAddress().WithMessage(ContractsRes.Models_Api_UserViewModelBase_Email_Invalid);
        }
    }
}