using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;
using Tauron.Application.Composition;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Views.Core;

namespace Tauron.Application.Views
{
    [Export(typeof(ViewManager))]
    [PublicAPI]
    public sealed class ViewManager : IViewLocator
    {
        public ViewManager()
        {
            ViewLocator = Factory.Object<AttributeBasedLocator>();
        }

        public ViewManager([NotNull] IViewLocator locator)
        {
            if (locator == null) throw new ArgumentNullException(nameof(locator));
            ViewLocator = locator;
        }

        [NotNull]
        public IViewLocator ViewLocator { get; set; }

        [NotNull]
        public static ViewManager Manager => CompositionServices.Container.Resolve<ViewManager>();


        public IWindow CreateWindow(string name, params object[] parameters)
        {
            return ViewLocator.CreateWindow(name, parameters);
        }

        public Type GetViewType(string name)
        {
            return ViewLocator.GetViewType(name);
        }

        public DependencyObject CreateView(string name)
        {
            return ViewLocator.CreateView(name);
        }

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            return ViewLocator.GetAllViews(name);
        }

        public void Register(ExportNameHelper export)
        {
            ViewLocator.Register(export);
        }

        public DependencyObject CreateViewForModel(object model)
        {
            return ViewLocator.CreateViewForModel(model);
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            return ViewLocator.CreateViewForModel(model);
        }

        public string GetName(ViewModelBase model)
        {
            return ViewLocator.GetName(model);
        }

        [CanBeNull]
        public TType CreateView<TType>([NotNull] string name)
            where TType : class
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            return CreateView(name) as TType;
        }

        [CanBeNull]
        public IWindow GetWindow([NotNull] string windowName)
        {
            return UiSynchronize.Synchronize.Invoke(() =>
            {
                var wind =
                    System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.Name == windowName);
                return wind == null ? null : new WpfWindow(wind);
            });
        }

        #region ExportView

        public static readonly DependencyProperty ExportViewProperty =
            DependencyProperty.RegisterAttached("ExportView", typeof(string), typeof(ViewManager),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions
                        .NotDataBindable,
                    ExportViewPropertyChanged));

        private static void ExportViewPropertyChanged([NotNull] DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs
                dependencyPropertyChangedEventArgs)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            var manager = Manager;
            var name = dependencyPropertyChangedEventArgs.NewValue as string;

            if (string.IsNullOrWhiteSpace(name) || dependencyObject as Control == null) return;

            // ReSharper disable once AssignNullToNotNullAttribute
            manager.Register(new ExportNameHelper(name, dependencyObject));
        }

        public static void SetExportView([NotNull] DependencyObject dependencyObject, [NotNull] string name)
        {
            dependencyObject.SetValue(ExportViewProperty, name);
        }

        #endregion

        #region Import View

        public static readonly DependencyProperty ImportViewProperty =
            DependencyProperty.RegisterAttached("ImportView", typeof(string), typeof(ViewManager),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions
                        .NotDataBindable,
                    ImportViewPropertyChangedCallback));

        private static void ImportViewPropertyChangedCallback([NotNull] DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs
                dependencyPropertyChangedEventArgs)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            //DEBUG var name = dependencyObject.GetValue(FrameworkElement.NameProperty);
            var viewName = (string) dependencyPropertyChangedEventArgs.NewValue;

            var panel = dependencyObject as Panel;
            if (panel != null)
            {
                panel.Children.Clear();
                foreach (var control in Manager.GetAllViews(viewName)) panel.Children.Add((Control) control);
                return;
            }

            var itemsCon = dependencyObject as ItemsControl;
            if (itemsCon != null)
            {
                itemsCon.Items.Clear();
                foreach (var control in Manager.GetAllViews(viewName)) itemsCon.Items.Add(control);
                return;
            }

            var contentControl = dependencyObject as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = Manager.CreateView(viewName);
                return;
            }

            var decorator = dependencyObject as Decorator;
            if (decorator != null)
            {
                decorator.Child = (UIElement) Manager.CreateView(viewName);
                return;
            }

            var propertyes = TypeDescriptor.GetProperties(dependencyObject);
            var attribute = dependencyObject.GetType().GetCustomAttribute<ContentPropertyAttribute>();
            PropertyDescriptor desc;

            if (attribute != null)
            {
                desc = propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == attribute.Name);
            }
            else
            {
                var altName = dependencyObject.GetValue(ContentPropertyProperty) as string;
                desc = !string.IsNullOrEmpty(altName)
                    ? propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == altName)
                    : null;
            }

            var viewType = Manager.GetViewType(viewName);

            if (desc == null || !desc.PropertyType.IsAssignableFrom(viewType)) return;

            desc.SetValue(dependencyObject, Manager.CreateView(viewName));
        }

        public static void SetImportView([NotNull] DependencyObject dependencyObject, [NotNull] string value)
        {
            dependencyObject.SetValue(ImportViewProperty, value);
        }

        #endregion

        #region Content Property

        public static readonly DependencyProperty ContentPropertyProperty =
            DependencyProperty.RegisterAttached("ContentProperty", typeof(string), typeof(ViewManager),
                new FrameworkPropertyMetadata("Content",
                    FrameworkPropertyMetadataOptions.Inherits |
                    FrameworkPropertyMetadataOptions
                        .NotDataBindable));

        [CanBeNull]
        internal static string GetContentProperty([NotNull] DependencyObject element)
        {
            return (string) element.GetValue(ContentPropertyProperty);
        }

        public static void SetContentProperty([NotNull] DependencyObject dependencyObject, [NotNull] string value)
        {
            dependencyObject.SetValue(ContentPropertyProperty, value);
        }

        #endregion

        #region SortOrder

        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.RegisterAttached("SortOrder", typeof(int), typeof(ViewManager),
                new PropertyMetadata(int.MaxValue));

        public static void SetSortOrder([NotNull] DependencyObject element, int value)
        {
            element.SetValue(SortOrderProperty, value);
        }

        public static int GetSortOrder([NotNull] DependencyObject element)
        {
            return (int) element.GetValue(SortOrderProperty);
        }

        #endregion
    }
}