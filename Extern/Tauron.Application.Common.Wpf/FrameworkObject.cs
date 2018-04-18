#region

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The framework object.</summary>
    [DebuggerStepThrough]
    [PublicAPI]
    public sealed class FrameworkObject : IWeakReference
    {
        [DebuggerStepThrough]
        private class ElementReference<TReference> : IWeakReference
            where TReference : class
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ElementReference{TReference}" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ElementReference{TReference}" /> Klasse.
            /// </summary>
            /// <param name="reference">
            ///     The reference.
            /// </param>
            /// <param name="isWeak">
            ///     The is weak.
            /// </param>
            public ElementReference([NotNull] TReference reference, bool isWeak)
            {
                if (reference == null) throw new ArgumentNullException(nameof(reference));

                if (isWeak) _weakRef = new WeakReference<TReference>(reference);
                else _reference      = reference;
            }

            #endregion

            #region Fields

            private readonly TReference _reference;

            private readonly WeakReference<TReference> _weakRef;

            #endregion

            #region Public Properties

            /// <summary>Gets the target.</summary>
            [NotNull]
            public TReference Target => _weakRef != null ? _weakRef.TypedTarget() : _reference;

            /// <summary>Gets a value indicating whether is alive.</summary>
            public bool IsAlive => _weakRef == null || _weakRef.IsAlive();

            #endregion
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FrameworkObject" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="FrameworkObject" /> Klasse.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="isWeak">
        ///     The is weak.
        /// </param>
        public FrameworkObject([CanBeNull] object obj, bool isWeak = true)
        {
            var fe  = obj as FrameworkElement;
            var fce = obj as FrameworkContentElement;

            _isFe   = fe != null;
            _isFce  = fce != null;
            IsValid = _isFce || _isFe;

            // ReSharper disable AssignNullToNotNullAttribute
            if (_isFe) _fe        = new ElementReference<FrameworkElement>(fe, isWeak);
            else if (_isFce) _fce = new ElementReference<FrameworkContentElement>(fce, isWeak);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        #endregion

        #region Explicit Interface Properties

        bool IWeakReference.IsAlive
        {
            get
            {
                if (_isFe) return _fe.IsAlive;

                return _isFce && _fce.IsAlive;
            }
        }

        #endregion

        #region Fields

        private readonly ElementReference<FrameworkContentElement> _fce;

        private readonly ElementReference<FrameworkElement> _fe;

        private readonly bool _isFce;

        private readonly bool _isFe;

        #endregion

        #region Public Events

        /// <summary>The data context changed.</summary>
        public event DependencyPropertyChangedEventHandler DataContextChanged
        {
            add
            {
                if (!IsValid) return;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) fe.DataContextChanged               += value;
                else if (TryGetFrameworkContentElement(out fce)) fce.DataContextChanged += value;
            }

            remove
            {
                if (!IsValid) return;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) fe.DataContextChanged               -= value;
                else if (TryGetFrameworkContentElement(out fce)) fce.DataContextChanged -= value;
            }
        }

        /// <summary>The loaded event.</summary>
        public event RoutedEventHandler LoadedEvent
        {
            add
            {
                if (!IsValid) return;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) fe.Loaded               += value;
                else if (TryGetFrameworkContentElement(out fce)) fce.Loaded += value;
            }

            remove
            {
                if (!IsValid) return;

                FrameworkElement        fe;
                FrameworkContentElement fce;
                if (TryGetFrameworkElement(out fe)) fe.Loaded               -= value;
                else if (TryGetFrameworkContentElement(out fce)) fce.Loaded -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the data context.</summary>
        [CanBeNull]
        public object DataContext
        {
            get
            {
                if (!IsValid) return null;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) return fe.DataContext;

                return TryGetFrameworkContentElement(out fce) ? fce.DataContext : null;
            }

            set
            {
                if (!IsValid) return;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) fe.DataContext               = value;
                else if (TryGetFrameworkContentElement(out fce)) fce.DataContext = value;
            }
        }

        /// <summary>Gets a value indicating whether is valid.</summary>
        public bool IsValid { get; }

        /// <summary>Gets the original.</summary>
        [CanBeNull]
        public DependencyObject Original
        {
            get
            {
                if (_isFe) return _fe.Target;

                return _isFce ? _fce.Target : null;
            }
        }

        /// <summary>Gets the parent.</summary>
        [CanBeNull]
        public DependencyObject Parent
        {
            get
            {
                if (!IsValid) return null;

                FrameworkElement        fe;
                FrameworkContentElement fce;

                if (TryGetFrameworkElement(out fe)) return fe.Parent;

                return TryGetFrameworkContentElement(out fce) ? fce.Parent : null;
            }
        }

        /// <summary>Gets the visual parent.</summary>
        [CanBeNull]
        public DependencyObject VisualParent
        {
            get
            {
                if (!IsValid) return null;

                FrameworkElement fe;

                return TryGetFrameworkElement(out fe) ? VisualTreeHelper.GetParent(fe) : null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The try get framework content element.
        /// </summary>
        /// <param name="contentElement">
        ///     The content element.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool TryGetFrameworkContentElement(out FrameworkContentElement contentElement)
        {
            contentElement = _isFce ? _fce.Target : null;

            return contentElement != null;
        }

        /// <summary>
        ///     The try get framework element.
        /// </summary>
        /// <param name="frameworkElement">
        ///     The framework element.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool TryGetFrameworkElement(out FrameworkElement frameworkElement)
        {
            frameworkElement = _isFe ? _fe.Target : null;

            return frameworkElement != null;
        }

        #endregion
    }
}