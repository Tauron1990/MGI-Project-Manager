using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ReactiveUI;
using Tauron.Reactive.Procedure.Wpf.Propertys;

namespace Tauron.Reactive.Procedure.Wpf
{
    [PublicAPI]
    public class ProcedureObject : ReactiveObject, IDisposable
    {
        private bool _disposed;
        private readonly ConcurrentDictionary<string, IReactiveProperty> _properties = new ConcurrentDictionary<string, IReactiveProperty>();

        protected TType GetValue<TType>(string name)
        {
            if (_properties.TryGetValue(name, out var prop))
            {
                
            }
        }

        protected IObserver<TType> RegisterProperty<TType>(string name, SubjectBase<TType> subject = null)
        {
            var prop = new PropertySubject<TType>(subject ?? new Subject<TType>(), this, name);

            if (_properties.TryAdd(name, prop))
                return prop;

            throw new InvalidOperationException("Duplicate Name");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void CheckDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException(nameof(ProcedureObject));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _disposed = true;
            foreach (var reactiveProperty in _properties)
                reactiveProperty.Value?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}