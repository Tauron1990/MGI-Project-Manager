using System;
using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Views.Core;

namespace Tauron.Application.Views
{
    public sealed class ExportNameHelper : ISortableViewExportMetadata
    {
        private readonly DependencyObject _dependencyObject;

        public ExportNameHelper([NotNull] string name, [NotNull] DependencyObject dependencyObject)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            _dependencyObject = dependencyObject;
            Name = name;
            Order = ViewManager.GetSortOrder(dependencyObject);
        }

        public string Name { get; }

        public int Order { get; }

        [NotNull]
        public object GetMeta()
        {
            return this;
        }

        [NotNull]
        public object GetValue()
        {
            return _dependencyObject;
        }
    }
}