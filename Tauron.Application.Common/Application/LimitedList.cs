using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public class LimitedList<T> : Collection<T>
    {
        private int _limit;
        
        public LimitedList() : this(-1) {}
        public LimitedList(int limit) => _limit = limit;

        public int Limit
        {
            get => _limit;

            set
            {
                _limit = value;
                CleanUp();
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            CleanUp();
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            CleanUp();
        }
        
        private void CleanUp()
        {
            if (Limit == -1) return;

            while (Count > Limit) Items.RemoveAt(0);
        }
    }
}