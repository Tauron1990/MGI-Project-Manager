#region

using System;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public delegate bool InterceptorCallback(ref object value);

    [PublicAPI]
    public class SimpleResolver : IResolver
    {
        private class ExportFactoryHelper
        {
            private readonly ExportMetadata _buildMetadata;
            private readonly IContainer _container;
            private readonly IResolverExtension[] _extensions;
            private readonly InterceptorCallback _interceptor;
            private readonly object _metadataObject;

            public ExportFactoryHelper([NotNull] IContainer container, [NotNull] ExportMetadata buildMetadata,
                [NotNull] object metadataObject, [CanBeNull] InterceptorCallback interceptor,
                [NotNull] IResolverExtension[] extensions)
            {
                _container = container;
                _buildMetadata = buildMetadata;
                _metadataObject = metadataObject;
                _interceptor = interceptor;
                _extensions = extensions;
            }

            [CanBeNull]
            public object BuildUp([CanBeNull] BuildParameter[] parameters)
            {
                return BuildUp(parameters, null);
            }

            [CanBeNull]
            public object BuildUp([CanBeNull] BuildParameter[] parameters, [CanBeNull] ErrorTracer error)
            {
                if (error == null)
                    error = new ErrorTracer();

                var temp = _container.BuildUp(_buildMetadata, error, parameters);
                if (error.Exceptional) throw new BuildUpException(error);

                var effectiveType = temp.GetType();
                var extension = _extensions.FirstOrDefault(e => e.TargetType == effectiveType);
                if (extension != null)
                    temp = extension.Progress(_buildMetadata, temp);

                var flag = _interceptor == null || _interceptor(ref temp);

                return !flag ? null : temp;
            }

            [NotNull]
            public object Metadata()
            {
                return _metadataObject;
            }
        }

        #region Constructors and Destructors

        public SimpleResolver([NotNull] ExportMetadata metadata, [NotNull] IContainer container,
            bool isExportFactory, [CanBeNull] Type factoryType, [CanBeNull] object metadataObject,
            [CanBeNull] Type metadataType, [CanBeNull] InterceptorCallback interceptor, bool isDescriptor,
            [NotNull] IResolverExtension[] extensions)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            _isExportFactory = isExportFactory;
            _factoryType = factoryType;
            _metadataObject = metadataObject;
            _metadataType = metadataType;
            _interceptor = interceptor;
            _isDescriptor = isDescriptor;
            _extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
        }

        #endregion

        #region Public Methods and Operators

        public object Create([NotNull] ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Import For " + Metadata;

            var helper = new ExportFactoryHelper(Container, Metadata, _metadataObject, _interceptor, _extensions);

            try
            {
                if (_isDescriptor) return new ExportDescriptor(Metadata);

                if (_isExportFactory)
                    return
                        Activator.CreateInstance(
                            typeof(InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType),
                            new Func<BuildParameter[], object>(helper.BuildUp),
                            new Func<object>(helper.Metadata), Metadata.Export.ImplementType);
                try
                {
                    errorTracer.IncrementIdent();
                    return helper.BuildUp(null, errorTracer);
                }
                finally
                {
                    errorTracer.DecrementIdent();
                }
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion

        #region Fields

        private readonly Type _factoryType;
        private readonly bool _isExportFactory;
        private readonly object _metadataObject;
        private readonly Type _metadataType;
        private readonly InterceptorCallback _interceptor;
        private readonly bool _isDescriptor;
        private readonly IResolverExtension[] _extensions;

        #endregion

        #region Public Properties

        /// <summary>Gets the container.</summary>
        [NotNull]
        public IContainer Container { get; }

        /// <summary>Gets the metadata.</summary>
        [NotNull]
        public ExportMetadata Metadata { get; }

        #endregion
    }
}