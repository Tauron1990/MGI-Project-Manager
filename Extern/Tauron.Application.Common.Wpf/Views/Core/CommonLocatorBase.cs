using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    public abstract class CommonLocatorBase : IViewLocator
    {
        private Dictionary<string, ExportNameHelper> _views = new Dictionary<string, ExportNameHelper>();

        public void Register(ExportNameHelper export)
        {
            _views[export.Name] = export;
        }

        public DependencyObject CreateViewForModel(object model)
        {
            var temp = CreateViewForModel(model.GetType());
            new FrameworkObject(temp, false).DataContext = model;

            return temp;
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            var name = GetName(model);
            if (name == null) return null;

            var temp = NamingHelper.CreatePossibilyNames(name)
                .Select(Match)
                .FirstOrDefault(view => view != null);

            if (temp != null) return temp;

            temp = _views.First(v => v.Key == name).Value.GetValue() as DependencyObject;
            if (temp is Window) temp = null;

            return temp;
        }

        public DependencyObject CreateView(string name)
        {
            return Match(name);
        }

        public IWindow CreateWindow(string name, object[] parameters)
        {
            if (!_views.TryGetValue(name, out var export)) return CreateWindowImpl(name, parameters);

            return export.GetValue() is Window win ? new WpfWindow(win) : CreateWindowImpl(name, parameters);
        }

        public abstract Type GetViewType(string name);

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            return GetAllViewsImpl(name).OrderBy(meta => meta.Metadata.Order).Select(i => i.Resolve());
        }

        public string GetName(ViewModelBase model)
        {
            return GetName(model.GetType());
        }


        [NotNull]
        public abstract DependencyObject Match([NotNull] string name);

        [CanBeNull]
        public abstract string GetName([NotNull] Type model);

        [CanBeNull]
        public abstract DependencyObject Match([NotNull] ISortableViewExportMetadata name);

        [NotNull]
        public abstract IEnumerable<InstanceResolver<Control, ISortableViewExportMetadata>> GetAllViewsImpl([NotNull] string name);

        [NotNull]
        public abstract IWindow CreateWindowImpl([NotNull] string name, [CanBeNull] object[] parameters);
    }
}