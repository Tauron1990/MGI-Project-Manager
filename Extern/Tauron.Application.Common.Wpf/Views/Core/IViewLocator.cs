using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    [PublicAPI]
    public interface IViewLocator
    {
        void Register([NotNull] ExportNameHelper export);

        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] object model);

        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] Type model);

        [NotNull]
        DependencyObject CreateView([NotNull] string name);

        [NotNull]
        IWindow CreateWindow([NotNull] string name, [CanBeNull] object[] parameters);

        [NotNull]
        Type GetViewType([NotNull] string name);

        [NotNull]
        IEnumerable<DependencyObject> GetAllViews([NotNull] string name);

        [CanBeNull]
        string GetName([NotNull] ViewModelBase model);
    }
}