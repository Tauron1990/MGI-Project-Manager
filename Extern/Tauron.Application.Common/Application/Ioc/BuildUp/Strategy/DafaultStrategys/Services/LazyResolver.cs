#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The lazy resolver.</summary>
    public class LazyResolver : IResolver
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LazyResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="LazyResolver" /> Klasse.
        /// </summary>
        /// <param name="resolver">
        ///     The resolver.
        /// </param>
        /// <param name="lazy">
        ///     The lazy.
        /// </param>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        public LazyResolver(SimpleResolver resolver, Type lazy, IMetadataFactory factory)
        {
            _resolver = resolver;
            _lazy = lazy;
            _factory = factory;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Create(ErrorTracer errorTracer)
        {
            return CreateLazy(
                _lazy,
                _factory,
                _resolver.Metadata.Metadata ?? new Dictionary<string, object>(),
                _resolver, errorTracer);
        }

        #endregion

        #region Methods

        private static object CreateLazy(
            [NotNull] Type lazytype,
            [NotNull] IMetadataFactory metadataFactory,
            [NotNull] IDictionary<string, object> metadataValue,
            [NotNull] SimpleResolver creator, [NotNull] ErrorTracer errorTracer)
        {
            if (lazytype == null) throw new ArgumentNullException(nameof(lazytype));
            if (metadataFactory == null) throw new ArgumentNullException(nameof(metadataFactory));
            if (metadataValue == null) throw new ArgumentNullException(nameof(metadataValue));
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (errorTracer == null) throw new ArgumentNullException(nameof(errorTracer));
            errorTracer.Phase = "Injecting Lazy For " + lazytype.Name;

            try
            {
                var openGeneric = lazytype.GetGenericTypeDefinition();

                var trampolineBase = typeof(LazyTrampoline<>);
                var trampolineGenerics = new Type[1];
                trampolineGenerics[0] = lazytype.GenericTypeArguments[0];

                var trampoline = trampolineBase.MakeGenericType(trampolineGenerics);

                var trampolineImpl = (LazyTrampolineBase) Activator.CreateInstance(trampoline, creator);
                var metadata = openGeneric == InjectorBaseConstants.Lazy ? null : lazytype.GenericTypeArguments[1];

                if (metadata == null) return Activator.CreateInstance(lazytype, trampolineImpl.CreateFunc());

                return Activator.CreateInstance(
                    lazytype,
                    trampolineImpl.CreateFunc(),
                    metadataFactory.CreateMetadata(metadata, metadataValue));
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion

        /// <summary>
        ///     The lazy trampoline.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        private class LazyTrampoline<T> : LazyTrampolineBase
        {
            #region Fields

            /// <summary>The _resolver.</summary>
            private readonly SimpleResolver _resolver;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LazyTrampoline{T}" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LazyTrampoline{T}" /> Klasse.
            ///     Initializes a new instance of the <see cref="LazyTrampoline{T}" /> class.
            /// </summary>
            /// <param name="resolver">
            ///     The resolver.
            /// </param>
            public LazyTrampoline([NotNull] SimpleResolver resolver)
            {
                _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The create func.</summary>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            public override object CreateFunc()
            {
                return (Func<T>) Create;
            }

            #endregion

            #region Methods

            /// <summary>The create.</summary>
            /// <returns>
            ///     The <see cref="T" />.
            /// </returns>
            private T Create()
            {
                return (T) _resolver.Create(new ErrorTracer());
            }

            #endregion
        }

        private abstract class LazyTrampolineBase
        {
            #region Public Methods and Operators

            /// <summary>The create func.</summary>
            /// <returns>
            ///     The <see cref="object" />.
            /// </returns>
            public abstract object CreateFunc();

            #endregion
        }

        #region Fields

        private readonly IMetadataFactory _factory;

        private readonly Type _lazy;

        private readonly SimpleResolver _resolver;

        #endregion
    }
}