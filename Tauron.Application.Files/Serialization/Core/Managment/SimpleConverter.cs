using System;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public abstract class SimpleConverter<TTarget>
    {
        [CanBeNull]
        public abstract object ConvertBack(TTarget target);

        public abstract TTarget Convert([CanBeNull] object source);

        [CanBeNull]
        public virtual Exception VerifyError() => null;
    }
}