using FluentValidation;
using Tauron.Application.MgiProjectManager.Server.Data.Api;

namespace Tauron.Application.MgiProjectManager.Server.Data.Validators
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