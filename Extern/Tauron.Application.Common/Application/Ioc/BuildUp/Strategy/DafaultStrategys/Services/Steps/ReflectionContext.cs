using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    [PublicAPI]
    public class ReflectionContext
    {
        private readonly InjectorContext _parentContext;

        public ReflectionContext([NotNull] IMetadataFactory metadataFactory, [NotNull] Type memberType,
            [NotNull] InjectorContext parentContext, [NotNull] IResolverExtension[] resolverExtensions)
        {
            _parentContext = parentContext;
            MetadataFactory = metadataFactory;
            MemberType = memberType;
            ResolverExtensions = resolverExtensions;
            CurrentType = memberType;
            BuildParametersRegistry = new ExportRegistry();
        }

        public IResolverExtension[] ResolverExtensions { get; private set; }

        [CanBeNull]
        public ExportMetadata ExportMetadataOverride { get; set; }

        [NotNull]
        public ExportRegistry BuildParametersRegistry { get; private set; }

        [NotNull]
        public IMetadataFactory MetadataFactory { get; private set; }

        [CanBeNull]
        public InterceptorCallback InterceptorCallback { get; set; }

        [NotNull]
        public Type MemberType { get; private set; }

        [NotNull]
        public Type CurrentType { get; set; }

        public Type AdditionalInfo { get; set; }

        public int Level { get; set; }

        [CanBeNull]
        public object Metadata { get; set; }

        [CanBeNull]
        public Type MetadataType { get; set; }

        //[NotNull]
        //public IResolver CreateSimpleListHelper([NotNull] ExportMetadata meta, bool isDescriptor)
        //{
        //    if (meta == null) throw new ArgumentNullException("meta");

        //    if (!CurrentType.IsGenericType)
        //        return new SimpleResolver(meta, _parentContext.Container, false, CurrentType, null, null, InterceptorCallback, isDescriptor);

        //    if (CurrentType.GetGenericTypeDefinition() != typeof (InstanceResolver<,>))
        //        return new SimpleResolver(meta, _parentContext.Container, false, CurrentType.GenericTypeArguments[0], null,null, InterceptorCallback, isDescriptor);

        //    Type metaType = CurrentType.GenericTypeArguments[1];
        //    object metaObject = MetadataFactory.CreateMetadata(metaType, meta.Metadata);

        //    return new SimpleResolver(meta, _parentContext.Container, true, CurrentType.GenericTypeArguments[0], metaObject,metaType, InterceptorCallback, isDescriptor);
        //}

        [CanBeNull]
        public IEnumerable<ExportMetadata> FindAllExports()
        {
            var type = _parentContext.Metadata.InterfaceType ?? ExtractRealType(CurrentType);
            var name = _parentContext.Metadata.ContractName;

            return
                BuildParametersRegistry.FindAll(type, name, new ErrorTracer())
                    .Union(
                        _parentContext.Container.FindExports(type, name, _parentContext.Tracer, Level),
                        new UionExportMetatdataEqualityComparer());
        }

        [CanBeNull]
        public ExportMetadata FindExport()
        {
            if (ExportMetadataOverride != null)
            {
                var exp = ExportMetadataOverride;
                ExportMetadataOverride = null;
                return exp;
            }

            var type = _parentContext.Metadata.InterfaceType ?? ExtractRealType(CurrentType);
            var name = _parentContext.Metadata.ContractName;

            var temp = BuildParametersRegistry.FindOptional(type, name, new ErrorTracer());
            if (temp != null) return temp;

            return _parentContext.Container.FindExport(_parentContext.Metadata.InterfaceType ?? ExtractRealType(CurrentType), _parentContext.Metadata.ContractName,
                _parentContext.Tracer, _parentContext.Metadata.Optional, Level);
        }

        [NotNull]
        private static Type ExtractRealType([NotNull] Type type)
        {
            if (!type.IsGenericType) return type.IsArray ? type.GetElementType() : type;

            var def = type.GetGenericTypeDefinition();

            if (def == typeof(InstanceResolver<,>) || def == typeof(Lazy<>) || def == typeof(Lazy<,>))
                return type.GenericTypeArguments[0];

            return type.IsArray ? type.GetElementType() : type;
        }

        private class UionExportMetatdataEqualityComparer : IEqualityComparer<ExportMetadata>
        {
            public bool Equals([NotNull] ExportMetadata x, [NotNull] ExportMetadata y)
            {
                return x.Export.ImplementType == y.Export.ImplementType;
            }

            public int GetHashCode([NotNull] ExportMetadata obj)
            {
                return obj.Export.ImplementType?.GetHashCode() ?? obj.GetHashCode();
            }
        }
    }
}