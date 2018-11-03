// The file TogglrButtonList.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="TogglrButtonList.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The topic changed evnt args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The topic changed evnt args.</summary>
    public sealed class TopicChangedEvntArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopicChangedEvntArgs" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TopicChangedEvntArgs" /> Klasse.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        public TopicChangedEvntArgs([CanBeNull] object topic)
        {
            Topic = topic;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the topic.</summary>
        [CanBeNull]
        public object Topic { get; }

        #endregion
    }

    /// <summary>The toggle content button.</summary>
    public sealed class ToggleContentButton : ToggleButton, IToggle
    {
        #region Public Properties

        /// <summary>Gets or sets the topic.</summary>
        [CanBeNull]
        public object Topic { get; set; }

        #endregion

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
            Content = header;
        }

        /// <summary>The switch.</summary>
        public void Switch()
        {
            IsChecked = IsChecked != true;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The on checked.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            Switched(this, true);
        }

        /// <summary>
        ///     The on unchecked.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            Switched(this, false);
        }

        #endregion
    }

    /// <summary>The toggle button list.</summary>
    public class ToggleButtonList : ToggleSwitchBase<ToggleContentButton>
    {
        #region Static Fields

        public static readonly DependencyProperty TopicProperty = DependencyProperty.RegisterAttached(
            "Topic",
            typeof(object),
            typeof(
                ToggleButtonList
            ),
            new UIPropertyMetadata
                (null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="ToggleButtonList" /> class.
        ///     Initialisiert statische Member der <see cref="ToggleButtonList" /> Klasse.
        /// </summary>
        static ToggleButtonList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToggleButtonList),
                new FrameworkPropertyMetadata(typeof(ToggleButtonList)));
        }

        #endregion

        // Using a DependencyProperty as the backing store for Topic.  This enables animation, styling, binding, etc...

        #region Public Events

        /// <summary>The topic changed event.</summary>
        public event EventHandler<TopicChangedEvntArgs> TopicChangedEvent;

        /// <summary>The topic deactivatet.</summary>
        public event EventHandler TopicDeactivatet;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get topic.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public static object GetTopic([NotNull] DependencyObject obj)
        {
            return obj.GetValue(TopicProperty);
        }

        /// <summary>
        ///     The set topic.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetTopic([NotNull] DependencyObject obj, [NotNull] object value)
        {
            obj.SetValue(TopicProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The clear item.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void ClearItem([NotNull] ToggleContentButton toggle, [NotNull] object item)
        {
            toggle.Topic = null;
        }

        /// <summary>
        ///     The get header.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        protected override object GetHeader([NotNull] object item)
        {
            return item;
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
        [NotNull]
        protected override object GetItem([NotNull] ToggleContentButton toggle)
        {
            return toggle.Content;
        }

        /// <summary>
        ///     The item activateted.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        protected override void ItemActivateted([NotNull] ToggleContentButton toggle)
        {
            var handler = TopicChangedEvent;
            if (handler != null) handler(this, new TopicChangedEvntArgs(toggle.Topic));
        }

        /// <summary>
        ///     The item deactivatet.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        protected override void ItemDeactivatet([NotNull] ToggleContentButton toggle)
        {
            var handler = TopicDeactivatet;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The prepate item.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void PrepateItem([NotNull] ToggleContentButton toggle, [NotNull] object item)
        {
            var topic = GetTopic(toggle);
            if (topic == null)
            {
                var itemdo = item as DependencyObject;
                if (itemdo != null) topic = GetTopic(itemdo);
            }

            toggle.Topic = topic;
        }

        #endregion
    }
}