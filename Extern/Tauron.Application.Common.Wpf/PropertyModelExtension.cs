using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.Application.Models;

namespace Tauron.Application
{
    public class PropertyModelExtension : IContainerExtension
    {
        private class PropertyImportInterceptor : IImportInterceptor
        {
            private readonly ImportMetadata[] _metadatas;

            public PropertyImportInterceptor([NotNull] ImportMetadata[] metadatas)
            {
                _metadatas = metadatas;
            }

            public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
            {
                if (!_metadatas.Contains(metadata)) return true;

                var viewModel = (ViewModelBase) target;

                if (!(value is ModelBase model)) return true;

                viewModel.RegisterInheritedModel(metadata.ContractName, model);

                return true;
            }
        }

        private class InternalProxyService : IProxyService
        {
            public InternalProxyService()
            {
                var moduleScope = new ModuleScope();
                GenericGenerator = new ProxyGenerator(new DefaultProxyBuilder(moduleScope));
            }

            public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
            {
                interceptor = null;
                if (!typeof(ModelBase).IsAssignableFrom(metadata.Export.ImplementType)) return GenericGenerator;

                var targetImports =
                    imports.Where(meta => meta.Metadata.ContainsKey(EnablePropertyInheritanceMetadataName))
                        .Where(m => (bool) m.Metadata[EnablePropertyInheritanceMetadataName])
                        .ToArray();

                if (targetImports.Length == 0) return GenericGenerator;

                interceptor = new PropertyImportInterceptor(targetImports);

                return GenericGenerator;
            }

            public ProxyGenerator GenericGenerator { get; }
        }

        public const string EnablePropertyInheritanceMetadataName = "EnablePropertyInheritance";

        public void Initialize(ComponentRegistry components)
        {
            components.Register<IProxyService, InternalProxyService>(true);
        }

        [NotNull]
        private static string BuildImportName([NotNull] ImportMetadata metadata)
        {
            return metadata.InterfaceType + metadata.ContractName;
        }
    }
}