using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.Data
{
    [Serializable]
    public abstract class BaseEntity :INotifyPropertyChanged, INotifyPropertyChanging
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler  PropertyChanged;
        [field:NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        protected void SetWithNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return;

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [PublicAPI, Serializable]
    public abstract class GenericBaseEntity<TId> : BaseEntity
    {
        private TId _id;

        [Key]
        public TId Id
        {
            get => _id;
            set => SetWithNotify(ref _id, value);
        }
    }
}