using System;
using FluentValidation;

namespace Tauron.Application.Deployment.Server.Services.Validatoren
{
    public sealed class DownloadEntryValidator : ValidatorBase<DownloadEntry, DownloadEntryValidator>
    {
        public DownloadEntryValidator() 
            => RuleFor(r => r.Url).Must(s => Uri.TryCreate(s, UriKind.Absolute, out _)).WithMessage("Url Kann nicht in eine Uri Umgewandelt Werden");

    }
}