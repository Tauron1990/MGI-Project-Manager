#region

using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    /// <summary>
    ///     The brush value converter.
    /// </summary>
    public sealed class BrushValueConverter : ValueConverterFactoryBase
    {
        private class Converter : ValueConverterBase<string, Brush>
        {
            #region Static Fields

            private static readonly BrushConverter ConverterImpl = new BrushConverter();

            #endregion

            #region Methods

            /// <summary>
            ///     The convert.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="Brush" />.
            /// </returns>
            [CanBeNull]
            protected override Brush Convert([NotNull] string value)
            {
                return ConverterImpl.ConvertFrom(value) as Brush;
            }

            #endregion
        }

        #region Methods

        /// <summary>
        ///     The create.
        /// </summary>
        /// <returns>
        ///     The <see cref="IValueConverter" />.
        /// </returns>
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        #endregion
    }
}