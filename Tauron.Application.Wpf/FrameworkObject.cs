#region

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Tauron.Application.Wpf.Helper;

#endregion

namespace Tauron.Application.Wpf
{
    /// <summary>The framework object.</summary>
    [DebuggerStepThrough]
    [PublicAPI]
    public sealed class FrameworkObject : IInternalWeakReference
    {
        private readonly ElementReference<FrameworkContentElement>? _fce;

        private readonly ElementReference<FrameworkElement>? _fe;

        private readonly bool _isFce;

        private readonly bool _isFe;

        public FrameworkObject(object? obj, bool isWeak = true)
        {
            var fe = obj as FrameworkElement;
            var fce = obj as FrameworkContentElement;

            _isFe = fe != null;
            _isFce = fce != null;
            IsValid = _isFce || _isFe;

            // ReSharper disable AssignNullToNotNullAttribute
            if (fe != null) _fe = new ElementReference<FrameworkElement>(fe, isWeak);
            else if (fce != null) _fce = new ElementReference<FrameworkContentElement>(fce, isWeak);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        public object? DataContext
        {
            get
            {
                if (!IsValid) return null;

                if (TryGetFrameworkElement(out var fe)) return fe.DataContext;
                return TryGetFrameworkContentElement(out var fce) ? fce.DataContext : null;
            }

            set
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContext = value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContext = value;
            }
        }

        public bool IsValid { get; }

        public DependencyObject? Original
        {
            get
            {
                if (_isFe) return _fe?.Target;
                return _isFce ? _fce?.Target : null;
            }
        }

        public DependencyObject? Parent
        {
            get
            {
                if (!IsValid) return null;

                if (TryGetFrameworkElement(out var fe)) return fe.Parent;
                return TryGetFrameworkContentElement(out var fce) ? fce.Parent : null;
            }
        }

        public DependencyObject? VisualParent
        {
            get
            {
                if (!IsValid) return null;

                return TryGetFrameworkElement(out var fe) ? VisualTreeHelper.GetParent(fe) : null;
            }
        }

        bool IInternalWeakReference.IsAlive
        {
            get
            {
                if (_isFe) return _fe?.IsAlive ?? false;

                return _isFce && (_fce?.IsAlive ?? false);
            }
        }

        public event DependencyPropertyChangedEventHandler DataContextChanged
        {
            add
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContextChanged += value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContextChanged += value;
            }

            remove
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.DataContextChanged -= value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.DataContextChanged -= value;
            }
        }

        public event RoutedEventHandler LoadedEvent
        {
            add
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.Loaded += value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.Loaded += value;
            }

            remove
            {
                if (!IsValid) return;

                if (TryGetFrameworkElement(out var fe)) fe.Loaded -= value;
                else if (TryGetFrameworkContentElement(out var fce)) fce.Loaded -= value;
            }
        }

        public bool TryGetFrameworkContentElement([NotNullWhen(true)] out FrameworkContentElement? contentElement)
        {
            contentElement = _isFce ? _fce?.Target : null;

            return contentElement != null;
        }

        public bool TryGetFrameworkElement([NotNullWhen(true)] out FrameworkElement? frameworkElement)
        {
            var temp = _isFe ? _fe?.Target : null;

            if (temp == null)
            {
                frameworkElement = null;
                return false;
            }

            frameworkElement = temp;
            return true;
        }

        [DebuggerStepThrough]
        private class ElementReference<TReference> : IInternalWeakReference
            where TReference : class
        {
            private readonly TReference? _reference;

            private readonly WeakReference<TReference>? _weakRef;

            public ElementReference([JetBrains.Annotations.NotNull] TReference reference, bool isWeak)
            {
                if (isWeak) _weakRef = new WeakReference<TReference>(Argument.NotNull(reference, nameof(reference)));
                else _reference = reference;
            }

            public TReference? Target => _weakRef != null ? _weakRef.TypedTarget() : _reference;

            public bool IsAlive => _weakRef == null || _weakRef.IsAlive();
        }
    }
}