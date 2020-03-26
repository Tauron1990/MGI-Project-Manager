using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.Shared
{
    public abstract class DIModule : IServiceCollection
    {
        internal IServiceCollection ServiceCollection { get; set; }

        IEnumerator<ServiceDescriptor> IEnumerable<ServiceDescriptor>.GetEnumerator() => ServiceCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) ServiceCollection).GetEnumerator();

        void ICollection<ServiceDescriptor>.Add(ServiceDescriptor item)
        {
            ServiceCollection.Add(item);
        }

        void ICollection<ServiceDescriptor>.Clear()
        {
            ServiceCollection.Clear();
        }

        public bool Contains(ServiceDescriptor item) => ServiceCollection.Contains(item);

        void ICollection<ServiceDescriptor>.CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            ServiceCollection.CopyTo(array, arrayIndex);
        }

        bool ICollection<ServiceDescriptor>.Remove(ServiceDescriptor item) => ServiceCollection.Remove(item);

        int ICollection<ServiceDescriptor>.Count => ServiceCollection.Count;

        bool ICollection<ServiceDescriptor>.IsReadOnly => ServiceCollection.IsReadOnly;

        int IList<ServiceDescriptor>.IndexOf(ServiceDescriptor item) => ServiceCollection.IndexOf(item);

        void IList<ServiceDescriptor>.Insert(int index, ServiceDescriptor item)
        {
            ServiceCollection.Insert(index, item);
        }

        void IList<ServiceDescriptor>.RemoveAt(int index)
        {
            ServiceCollection.RemoveAt(index);
        }

        ServiceDescriptor IList<ServiceDescriptor>.this[int index]
        {
            get => ServiceCollection[index];
            set => ServiceCollection[index] = value;
        }

        public abstract void Load();
    }
}