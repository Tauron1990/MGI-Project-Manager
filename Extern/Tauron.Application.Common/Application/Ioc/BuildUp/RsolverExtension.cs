using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp
{
    public interface IResolverExtension
    {
        [NotNull]
        Type TargetType { get; }

        [CanBeNull]
        object Progress([NotNull] ExportMetadata metadata, [NotNull] object export);
    }
}