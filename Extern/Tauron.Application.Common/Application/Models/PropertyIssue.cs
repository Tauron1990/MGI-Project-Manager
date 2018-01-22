using System;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public sealed class PropertyIssue
    {
        public PropertyIssue([NotNull] string propertyName, [CanBeNull] object value, [NotNull] string message)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (message == null) throw new ArgumentNullException(nameof(message));

            PropertyName = propertyName;
            Value = value;
            Message = message;
        }

        [NotNull]
        public string PropertyName { get; }

        [CanBeNull]
        public object Value { get; }

        [NotNull]
        public string Message { get; }
    }
}