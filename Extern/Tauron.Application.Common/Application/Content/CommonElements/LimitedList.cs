#region

using System.Collections.ObjectModel;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The limited list.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [PublicAPI]
    public class LimitedList<T> : Collection<T>
    {
        #region Fields

        /// <summary>The _limit.</summary>
        private int _limit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LimitedList{T}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="LimitedList{T}" /> Klasse.
        ///     Initializes a new instance of the <see cref="LimitedList{T}" /> class.
        /// </summary>
        public LimitedList()
        {
            Limit = -1;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the limit.</summary>
        /// <value>The limit.</value>
        public int Limit
        {
            get => _limit;

            set
            {
                _limit = value;
                CleanUp();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            CleanUp();
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
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            CleanUp();
        }

        /// <summary>The clean up.</summary>
        private void CleanUp()
        {
            if (Limit == -1) return;

            while (Count > Limit) Items.RemoveAt(0);
        }

        #endregion
    }
}