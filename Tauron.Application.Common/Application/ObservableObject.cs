using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application
{
    public static class PropertyHelper
    {
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            Argument.NotNull(propertyExpression, nameof(propertyExpression));
            var memberExpression = (MemberExpression) propertyExpression.Body;

            return memberExpression.Member.Name;
        }
    }

    [Serializable]
    [PublicAPI]
    [DebuggerStepThrough]
    public abstract class ObservableObject : INotifyPropertyChangedMethod
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetProperty<TType>(ref TType property, TType value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
        }

        public void SetProperty<TType>(ref TType property, TType value, Action changed, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<TType>.Default.Equals(property, value)) return;

            property = value;
            OnPropertyChangedExplicit(Argument.NotNull(name!, nameof(name)));
            changed();
        }
        
        // ReSharper disable once AssignNullToNotNullAttribute
        public virtual void OnPropertyChanged([CallerMemberName] string? eventArgs = null) => OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(eventArgs!, nameof(eventArgs))));

        public virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs) => OnPropertyChanged(this, Argument.NotNull(eventArgs, nameof(eventArgs)));

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs eventArgs) => 
            PropertyChanged?.Invoke(Argument.NotNull(sender, nameof(sender)), Argument.NotNull(eventArgs, nameof(eventArgs)));

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void OnPropertyChanged<T>(Expression<Func<T>> eventArgs) =>
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyHelper.ExtractPropertyName(Argument.NotNull(eventArgs, nameof(eventArgs)))));


        public virtual void OnPropertyChangedExplicit(string propertyName) => 
            OnPropertyChanged(new PropertyChangedEventArgs(Argument.NotNull(propertyName, nameof(propertyName))));
    }
}