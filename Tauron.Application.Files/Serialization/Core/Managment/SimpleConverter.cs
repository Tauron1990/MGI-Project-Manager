using System;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class SimpleConverter<TTarget>
    {
        public abstract object? ConvertBack(TTarget target);

        public abstract TTarget Convert(object? source);

        public virtual Exception? VerifyError()
        {
            return null;
        }
    }
}