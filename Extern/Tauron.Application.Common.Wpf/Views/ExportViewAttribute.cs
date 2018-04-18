using System;
using System.Windows.Controls;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.Application.Views.Core;

namespace Tauron.Application.Views
{
    [PublicAPI]
    [BaseTypeRequired(typeof(Control))]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ExportViewAttribute : ExportAttribute, ISortableViewExportMetadata
    {
        public ExportViewAttribute([NotNull] string viewName) : base(typeof(Control))
        {
            if (viewName == null) throw new ArgumentNullException(nameof(viewName));
            Name  = viewName;
            Order = int.MaxValue;
        }

        protected override LifetimeContextAttribute OverrideDefaultPolicy => new NotSharedAttribute();

        public override string DebugName => Name;

        protected override bool HasMetadata => true;

        public string Name { get; private set; }

        public int Order { get; set; }
    }
}