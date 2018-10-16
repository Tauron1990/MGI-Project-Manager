#region

using System.Windows;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The dependency object extensions.</summary>
    [PublicAPI]
    public static class DependencyObjectExtensions
    {
        #region Public Methods and Operators

        [CanBeNull]
        public static object FindResource([NotNull] this DependencyObject obj, [NotNull] object key)
        {
            var temp1 = obj as FrameworkElement;
            var temp2 = obj as FrameworkContentElement;
            object result = null;
            if (temp1 != null) result = temp1.TryFindResource(key);

            if (result == null && temp2 != null) result = temp2.TryFindResource(key);

            return result;
        }


        [CanBeNull]
        public static TType FindResource<TType>([NotNull] this DependencyObject obj, [NotNull] object key)
            where TType : class
        {
            return FindResource(obj, key) as TType;
        }

        #endregion
    }
}