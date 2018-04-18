// The file ExpanderList.cs is part of Tauron.Application.Common.Wpf.Controls.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf.Controls If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpanderList.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The toggle expander.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The toggle expander.</summary>
    public sealed class ToggleExpander : Expander, IToggle
    {
        #region Public Events

        /// <summary>The switched.</summary>
        public event Action<IToggle, bool> Switched;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The set header.
        /// </summary>
        /// <param name="header">
        ///     The header.
        /// </param>
        public void SetHeader([NotNull] object header)
        {
            Header = header;
        }

        /// <summary>The switch.</summary>
        public void Switch()
        {
            IsExpanded = !IsExpanded;
        }

        #endregion

        #region Methods

        /// <summary>The on collapsed.</summary>
        protected override void OnCollapsed()
        {
            base.OnCollapsed();
            Switched(this, false);
        }

        /// <summary>The on expanded.</summary>
        protected override void OnExpanded()
        {
            base.OnExpanded();
            Switched(this, true);
        }

        #endregion
    }

    /// <summary>The expander list.</summary>
    [PublicAPI]
    public class ExpanderList : ToggleSwitchBase<ToggleExpander>
    {
        #region Static Fields

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
                                                                                                       "Header",
                                                                                                       typeof(object),
                                                                                                       typeof(
                                                                                                           ExpanderList),
                                                                                                       new FrameworkPropertyMetadata
                                                                                                           (
                                                                                                            null,
                                                                                                            FrameworkPropertyMetadataOptions
                                                                                                                .AffectsRender |
                                                                                                            FrameworkPropertyMetadataOptions
                                                                                                                .AffectsMeasure));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="ExpanderList" /> class.
        ///     Initialisiert statische Member der <see cref="ExpanderList" /> Klasse.
        /// </summary>
        static ExpanderList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                                                     typeof(ExpanderList),
                                                     new FrameworkPropertyMetadata(typeof(ExpanderList)));
        }

        #endregion

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...

        #region Public Methods and Operators

        /// <summary>
        ///     The get header.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        public static object GetHeader([NotNull] DependencyObject obj)
        {
            return obj.GetValue(HeaderProperty);
        }

        /// <summary>
        ///     The set header.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetHeader([NotNull] DependencyObject obj, [NotNull] object value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get header.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        protected override object GetHeader([NotNull] object item)
        {
            var obj = item as DependencyObject;
            return obj == null ? null : GetHeader(obj);
        }

        /// <summary>
        ///     The get item.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        protected override object GetItem(ToggleExpander toggle)
        {
            return toggle.Content;
        }

        #endregion
    }
}