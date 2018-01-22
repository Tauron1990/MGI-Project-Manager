using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public abstract class BaseEntity :INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler  PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void SetWithNotify<T>(T value, ref T field, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return;

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [PublicAPI]
    public abstract class GenericBaseEntity<TId> : BaseEntity
    {
        private TId _id;

        [Key]
        public TId Id
        {
            get => _id;
            set => SetWithNotify(value, ref _id);
        }
    }
}