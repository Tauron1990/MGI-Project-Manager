using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tauron.Application.Composition;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    public class AttributeBasedLocator : CommonLocatorBase
    {
        [Inject] protected List<InstanceResolver<Control, ISortableViewExportMetadata>> Views;

        [Inject] protected List<InstanceResolver<Window, INameExportMetadata>> Windows;

        public override string GetName(Type model)
        {
            var attr = model.GetCustomAttribute<ExportViewModelAttribute>();
            return attr?.Name;
        }

        public override DependencyObject Match(ISortableViewExportMetadata name)
        {
            var temp = Views.FirstOrDefault(rs => rs.Metadata == name);

            return temp?.Resolve(true);
        }

        public override IEnumerable<InstanceResolver<Control, ISortableViewExportMetadata>> GetAllViewsImpl(string name)
        {
            return Views.Where(v => v.Metadata.Name == name);
        }

        public override DependencyObject Match(string name)
        {
            return Views.First(v => v.Metadata.Name == name).Resolve();
        }

        public override IWindow CreateWindowImpl(string name, object[] parameters)
        {
            try
            {
                BuildParameter[] buildParameters = null;
                if (parameters != null)
                {
                    buildParameters = new BuildParameter[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                        buildParameters[i] = new SimpleBuildPrameter(parameters[i]);
                }

                CompositionServices.BuildParameters = buildParameters;
                var window = Windows.First(win => win.Metadata.Name == name).ResolveRaw(true, buildParameters);

                return CastToWindow(window, name);
            }
            finally
            {
                CompositionServices.BuildParameters = null;
            }
        }

        public override Type GetViewType(string name)
        {
            return Views.First(vi => vi.Metadata.Name == name).RealType;
        }

        protected virtual IWindow CastToWindow(object objWindow, string name)
        {
            var window = (Window) objWindow;

            UiSynchronize.Synchronize.Invoke(() => window.Name = name);

            return new WpfWindow(window);
        }
    }
}