#region

using System.Windows;
using System.Windows.Controls;

#endregion

namespace Tauron.Application
{
    /// <summary>The alternativ template selector.</summary>
    public sealed class AlternativTemplateSelector : DataTemplateSelector
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The select template.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <returns>
        ///     The <see cref="DataTemplate" />.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (item == null || container == null) return null;

            // ReSharper restore HeuristicUnreachableCode
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            var key = item.GetType().Name;
            var ele = container.As<FrameworkElement>();

            var temp = ele?.TryFindResource(key).As<DataTemplate>();
            return temp;
        }

        #endregion
    }
}