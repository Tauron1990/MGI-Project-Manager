using System;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class MappingEntry<TContext>
        where TContext : IOrginalContextProvider
    {
        public abstract Exception? VerifyError();

        public abstract void Progress(object target, TContext context, SerializerMode mode);
    }
}