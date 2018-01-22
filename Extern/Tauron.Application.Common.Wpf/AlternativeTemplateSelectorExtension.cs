#region

using System;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The alternative template selector extension.</summary>
    [MarkupExtensionReturnType(typeof(DataTemplateSelector))]
    [PublicAPI]
    public sealed class AlternativeTemplateSelectorExtension : MarkupExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        ///     The service provider.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new AlternativTemplateSelector();
        }

        #endregion
    }
}