#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{

    [PublicAPI]
    public sealed class WorkspaceManager<TWorkspace> : UISyncObservableCollection<TWorkspace>
        where TWorkspace : class, ITabWorkspace
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkspaceManager{TWorkspace}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WorkspaceManager{TWorkspace}" /> Klasse.
        /// </summary>
        /// <param name="holder">
        ///     The holder.
        /// </param>
        public WorkspaceManager([NotNull] IWorkspaceHolder holder)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder));
            _holder = holder;
        }

        #endregion

        [NotNull]
        public ITabWorkspace ActiveItem

        {
            get => _activeItem;
            set
            {
                _activeItem = value;

                if (Equals(_activeItem, value)) return;

                _activeItem?.OnDeactivate();

                _activeItem = value;
                _activeItem.OnActivate();

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ActiveItem)));
            }
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The add range.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        public new void AddRange([NotNull] [ItemNotNull] IEnumerable<TWorkspace> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items.Where(it => it != null)) Add(item);
        }

        #endregion

        private void UnRegisterWorkspace([NotNull] ITabWorkspace space)
        {
            space.OnClose();
            _holder.UnRegister(space);
        }

        #region Fields

        private readonly IWorkspaceHolder _holder;
        private ITabWorkspace _activeItem;

        #endregion

        #region Methods

        /// <summary>The clear items.</summary>
        protected override void ClearItems()
        {
            foreach (var workspace in Items) UnRegisterWorkspace(workspace);

            base.ClearItems();
        }

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, [CanBeNull] TWorkspace item)
        {
            if (item == null) return;

            if (index < Count) UnRegisterWorkspace(this[index]);

            _holder.Register(item);

            base.InsertItem(index, item);
        }

        /// <summary>
        ///     The remove item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            UnRegisterWorkspace(this[index]);
            base.RemoveItem(index);
        }

        /// <summary>
        ///     The set item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void SetItem(int index, [CanBeNull] TWorkspace item)
        {
            if (item == null) return;

            UnRegisterWorkspace(this[index]);
            _holder.Register(item);
            base.SetItem(index, item);
        }

        #endregion
    }
}