using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    public sealed class ValidatorContext
    {
        public ValidatorContext([NotNull] ModelBase model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            Model = model;
            Items = new Dictionary<object, object>();
        }

        [NotNull] public ModelBase Model { get; }

        [NotNull] public ObservableProperty Property { get; internal set; }

        [NotNull] public Type ModelType => Model.GetType();

        [NotNull] public IDictionary<object, object> Items { get; }
    }
}