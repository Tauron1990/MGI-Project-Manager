#region

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class StringToIntConverter : ValueConverterFactoryBase
    {
        private class Converter : StringConverterBase<int>
        {
            protected override bool CanConvertBack => true;

            protected override string Convert(int value)
            {
                return value.ToString();
            }

            protected override int ConvertBack(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return 0;

                try
                {
                    return int.Parse(value);
                }
                catch (Exception e) when (e is ArgumentException || e is FormatException || e is OverflowException)
                {
                    return 0;
                }
            }
        }

        protected override IValueConverter Create()
        {
            return new Converter();
        }
    }


    /// <summary>The bool to visibility converter.</summary>
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class BoolToVisibilityConverter : ValueConverterFactoryBase
    {
        private class Converter : ValueConverterBase<bool, Visibility>
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Converter" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="Converter" /> Klasse.
            /// </summary>
            /// <param name="isHidden">
            ///     The is hidden.
            /// </param>
            /// <param name="reverse">
            ///     The reverse.
            /// </param>
            public Converter(bool isHidden, bool reverse)
            {
                _isHidden = isHidden;
                _reverse = reverse;
            }

            #endregion

            #region Properties

            /// <summary>Gets a value indicating whether can convert back.</summary>
            protected override bool CanConvertBack => true;

            #endregion

            #region Fields

            private readonly bool _isHidden;

            private readonly bool _reverse;

            #endregion

            #region Methods

            /// <summary>
            ///     The convert.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="Visibility" />.
            /// </returns>
            protected override Visibility Convert(bool value)
            {
                if (_reverse) value = !value;

                if (value) return Visibility.Visible;

                return _isHidden ? Visibility.Hidden : Visibility.Collapsed;
            }

            /// <summary>
            ///     The convert back.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            protected override bool ConvertBack(Visibility value)
            {
                bool result;
                switch (value)
                {
                    case Visibility.Collapsed:
                    case Visibility.Hidden:
                        result = false;
                        break;
                    case Visibility.Visible:
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }

                if (_reverse) result = !result;

                return result;
            }

            #endregion
        }

        #region Methods

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="IValueConverter" />.
        /// </returns>
        protected override IValueConverter Create()
        {
            return new Converter(IsHidden, Reverse);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is hidden.</summary>
        public bool IsHidden { get; set; }

        /// <summary>Gets or sets a value indicating whether reverse.</summary>
        public bool Reverse { get; set; }

        #endregion
    }
}