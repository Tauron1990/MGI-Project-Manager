#region

using System;
using System.Windows.Markup;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The simple localize with arg.</summary>
    [MarkupExtensionReturnType(typeof(string))]
    public class SimpleLocalizeWithArg : SimpleLocalize
    {
        #region Public Properties

        /// <summary>Gets the arg.</summary>
        [CanBeNull]
        public string Arg { get; private set; }

        #endregion

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
            var value = base.ProvideValue(serviceProvider) as string;

            if (value == null) return "null";

            if (value == string.Empty) return "null";

            try
            {
                return value.SFormat(Arg);
            }
            catch (FormatException)
            {
                return "null";
            }
        }

        #endregion
    }
}