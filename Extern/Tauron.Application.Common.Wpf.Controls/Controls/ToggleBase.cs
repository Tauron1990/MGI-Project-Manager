// The file ToggleBase.cs is part of Tauron.Application.Common.Wpf.Controls.
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
// <copyright file="ToggleBase.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The toggle switch selection base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Controls
{
    /// <summary>The toggle switch selection base.</summary>
    [PublicAPI]
    public abstract class ToggleSwitchSelectionBase : ItemsControl
    {
        #region Static Fields

        public static readonly DependencyProperty ActiveItemProperty = DependencyProperty.Register(
            "ActiveItem",
            typeof(object),
            typeof(
                ToggleSwitchSelectionBase
            ),
            new UIPropertyMetadata
            (null,
                OnPropertyChanged));

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the active item.</summary>
        public object ActiveItem
        {
            get => GetValue(ActiveItemProperty);

            set => SetValue(ActiveItemProperty, value);
        }

        #endregion

        // Using a DependencyProperty as the backing store for ActiveItem.  This enables animation, styling, binding, etc...

        #region Methods

        /// <summary>
        ///     The selection chagend.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected virtual void SelectionChagend(object item)
        {
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleSwitchSelectionBase) d).SelectionChagend(e.NewValue);
        }

        #endregion
    }

    /// <summary>
    ///     The toggle switch base.
    /// </summary>
    /// <typeparam name="TToggle">
    /// </typeparam>
    public abstract class ToggleSwitchBase<TToggle> : ToggleSwitchSelectionBase
        where TToggle : DependencyObject, IToggle, new()
    {
        #region Fields

        private readonly Dictionary<int, TToggle> controls;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToggleSwitchBase{TToggle}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ToggleSwitchBase{TToggle}" /> Klasse.
        /// </summary>
        protected ToggleSwitchBase()
        {
            controls = new Dictionary<int, TToggle>();
        }

        #endregion

        #region Properties

        /// <summary>Gets the active.</summary>
        protected IToggle Active { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The clear container for item override.
        /// </summary>
        /// <param name="element">
        ///     The element.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var toggle = (TToggle) element;
            foreach (var item2 in controls)
                if (ReferenceEquals(item, toggle))
                {
                    controls.Remove(item2.Key);
                    break;
                }

            ClearItem(toggle, item);
            toggle.SetHeader(null);
            toggle.Switched -= ToggleSwitched;
        }

        /// <summary>
        ///     The clear item.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected virtual void ClearItem(TToggle toggle, object item)
        {
        }

        // ReSharper disable VirtualMemberNeverOverriden.Global
        /// <summary>The create container.</summary>
        /// <returns>
        ///     The <see cref="TToggle" />.
        /// </returns>
        protected virtual TToggle CreateContainer()
        {
            // ReSharper restore VirtualMemberNeverOverriden.Global
            return new TToggle();
        }

        /// <summary>The get container for item override.</summary>
        /// <returns>
        ///     The <see cref="DependencyObject" />.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return CreateContainer();
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
        protected abstract object GetHeader(object item);

        /// <summary>
        ///     The get item.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        protected abstract object GetItem(TToggle toggle);

        /// <summary>
        ///     The is item its own container override.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TToggle || item is TextBlock;
        }

        /// <summary>
        ///     The item activateted.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        protected virtual void ItemActivateted(TToggle toggle)
        {
        }

        /// <summary>
        ///     The item deactivatet.
        /// </summary>
        /// <param name="toggle">
        ///     The toggle.
        /// </param>
        protected virtual void ItemDeactivatet(TToggle toggle)
        {
        }

        /// <summary>
        ///     The prepare container for item override.
        /// </summary>
        /// <param name="element">
        ///     The element.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var toggle = (TToggle) element;
            PrepateItem(toggle, item);
            controls[Items.IndexOf(item)] = toggle;
            var provider = item as IHeaderProvider;
            if (provider != null)
            {
                toggle.SetHeader(provider.Header);
            }
            else
            {
                var header = GetHeader(item);
                toggle.SetHeader(header ?? item.ToString());
            }

            toggle.Switched += ToggleSwitched;
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
        protected virtual void PrepateItem(TToggle toggle, object item)
        {
        }

        /// <summary>
        ///     The selection chagend.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void SelectionChagend(object item)
        {
            if (item == null && Active != null)
            {
                Active.Switch();
                return;
            }

            var control = controls[Items.IndexOf(item)];
            if (!Equals(control, Active)) control.Switch();
        }

        private void ToggleSwitched(IToggle sender, bool stade)
        {
            if (stade)
            {
                if (Active != null) Active.Switch();

                Active = sender;

                var item = GetItem((TToggle) Active);
                if (ActiveItem != null && ActiveItem != item) ActiveItem = item;

                ItemActivateted((TToggle) sender);
            }
            else
            {
                ItemDeactivatet((TToggle) Active);
                Active = null;
            }
        }

        #endregion
    }
}