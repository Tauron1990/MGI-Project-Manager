#region

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    /// <summary>The framework object.</summary>
    [DebuggerStepThrough]
    [PublicAPI]
    public sealed class FrameworkObject
    {
        public FrameworkObject(object? obj)
        {
            var fe = obj as FrameworkElement;
            var fce = obj as FrameworkContentElement;

            _isFe = fe != null;
            _isFce = fce != null;
            IsValid = _isFce || _isFe;

            // ReSharper disable AssignNullToNotNullAttribute
            if (fe != null) _fe = new ElementReference<FrameworkElement>(fe);
            else if (fce != null) _fce = new ElementReference<FrameworkContentElement>(fce);
            // ReSharper restore AssignNullToNotNullAttribute
        }
        
        [DebuggerStepThrough]
        private class ElementReference<TReference>
            where TReference : class
        {
            public ElementReference([JetBrains.Annotations.NotNull] TReference reference) 
                => Target = reference;

            [JetBrains.Annotations.NotNull]
            public TReference Target { get; }
        }

        private readonly ElementReference<FrameworkContentElement>? _fce;

        private readonly ElementReference<FrameworkElement>? _fe;

        private readonly bool _isFce;

        private readonly bool _isFe;

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
                if (_isFe && _fe != null) return _fe.Target;
                return _fce?.Target;
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

        public bool TryGetFrameworkContentElement([MaybeNullWhen(false)]out FrameworkContentElement contentElement)
        {
            contentElement = _isFce ? _fce?.Target! : null!;

            return contentElement != null;
        }

        public bool TryGetFrameworkElement([MaybeNullWhen(false)]out FrameworkElement frameworkElement)
        {
            frameworkElement = _isFe ? _fe?.Target! : null!;

            return frameworkElement != null;
        }
    }
}