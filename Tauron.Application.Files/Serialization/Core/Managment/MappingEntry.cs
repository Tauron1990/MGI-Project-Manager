using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class MappingEntry<TContext>
        where TContext : IOrginalContextProvider
    {
        [CanBeNull]
        public abstract Exception VerifyError();

        public abstract void Progress([NotNull] object target, [NotNull] TContext context, SerializerMode mode);
    }
}