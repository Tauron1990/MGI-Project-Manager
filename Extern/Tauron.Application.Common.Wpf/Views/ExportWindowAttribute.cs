using System;
using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Views
{
    [PublicAPI]
    [BaseTypeRequired(typeof(Window))]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ExportWindowAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportWindowAttribute([NotNull] string viewName)
            : base(typeof(Window))
        {
            if (viewName == null) throw new ArgumentNullException(nameof(viewName));
            Name = viewName;
        }

        public override string DebugName => Name;

        protected override LifetimeContextAttribute OverrideDefaultPolicy => new NotSharedAttribute();

        protected override bool HasMetadata => true;

        public string Name { get; private set; }
    }
}