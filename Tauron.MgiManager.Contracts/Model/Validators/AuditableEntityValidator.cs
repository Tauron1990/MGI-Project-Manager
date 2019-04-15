using FluentValidation;

namespace Tauron.MgiProjectManager.Model.Validators
{
    public class AuditableEntityValidator : AbstractValidator<AuditableEntity>
    {
        public AuditableEntityValidator()
        {
            RuleFor(e => e.CreatedBy).MaximumLength(256);
            RuleFor(e => e.UpdatedBy).MaximumLength(256);
        }
    }
}