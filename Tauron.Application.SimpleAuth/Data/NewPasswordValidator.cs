using FluentValidation;
using JetBrains.Annotations;

namespace Tauron.Application.SimpleAuth.Data
{
    [UsedImplicitly]
    public sealed class NewPasswordValidator : AbstractValidator<NewPasswordData>
    {
        public NewPasswordValidator()
        {
            RuleFor(d => d.NewPassword).NotEmpty();
            RuleFor(d => d.OldPassword).NotEmpty();
        }
    }
}