using FluentValidation;
using FluentValidation.Results;

namespace Tauron.Application.MgiProjectManager.Data.Api
{
    public class AppUserValidator : AbstractValidator<AppUser>
    {
        public AppUserValidator()
        {
            RuleFor(u => u.Email).EmailAddress().NotEmpty();
            RuleFor(u => u.Name).NotEmpty();
            RuleFor(u => u.Role).NotEmpty();
        }
    }
}