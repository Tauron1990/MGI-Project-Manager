using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public sealed class InstanceResolver<TExport, TMetadata>
        where TMetadata : class
    {
        private readonly Func<BuildParameter[], object> _resolver;

        private TMetadata _metadata;
        private Func<object> _metadataFactory;

        public InstanceResolver([NotNull] Func<BuildParameter[], object> resolver, [NotNull] Func<object> metadataFactory,
            [NotNull] Type realType)
        {
            RealType = realType ?? throw new ArgumentNullException(nameof(realType));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
        }


        [NotNull]
        public Type RealType { get; private set; }

        [NotNull]
        public TMetadata Metadata
        {
            get
            {
                if (_metadata != null) return _metadata;

                _metadata = (TMetadata) _metadataFactory();
                _metadataFactory = null;

                return _metadata;
            }
        }    

        public TExport Resolve([CanBeNull] BuildParameter[] buildParameters = null)
        {
            var obj = _resolver(buildParameters);

            if (obj == null) return default(TExport);

            return (TExport) obj;
        }

        public TExport Resolve(bool uiSync, [CanBeNull] BuildParameter[] buildParameters = null)
        {
            return uiSync
                ? UiSynchronize.Synchronize.Invoke(() => Resolve(buildParameters))
                : Resolve(buildParameters);
        }

        public object ResolveRaw([CanBeNull] BuildParameter[] buildParameters = null)
        {
            var obj = _resolver(buildParameters);

            return obj;
        }

        public object ResolveRaw(bool uiSync, [CanBeNull] BuildParameter[] buildParameters = null)
        {
            return uiSync
                ? UiSynchronize.Synchronize.Invoke(() => ResolveRaw(buildParameters))
                : ResolveRaw(buildParameters);
        }
    }
}