using System;
using System.Collections;
using System.Collections.Generic;

namespace Akka.Copy.Core
{
    public class ErrorProofEnumerable<TType> : IEnumerable<TType>
    {
        private class Enumerator : IEnumerator<TType>
        {
            private readonly IEnumerator<TType> _enumerator;
            private readonly Action<Exception>  _handler;

            public Enumerator(IEnumerator<TType> enumerator, Action<Exception> handler)
            {
                _enumerator = enumerator;
                _handler    = handler;
            }

            public void Dispose()
            {
                try
                {
                    _enumerator.Dispose();
                }
                catch (Exception e)
                {
                    _handler?.Invoke(e);
                }
            }

            public bool MoveNext()
            {
                try
                {
                    return _enumerator.MoveNext();
                }
                catch (Exception e)
                {
                    _handler?.Invoke(e);
                    return false;
                }
            }

            public void Reset()
            {
                try
                {
                    _enumerator.Reset();
                }
                catch (Exception e)
                {
                    _handler?.Invoke(e);
                }
            }

            public TType Current
            {
                get
                {
                    try
                    {
                        return _enumerator.Current;
                    }
                    catch (Exception e)
                    {
                        _handler?.Invoke(e);
                        return default(TType);
                    }
                }
            }

            object IEnumerator.Current => Current;
        }

        private readonly IEnumerable<TType> _enumerable;
        private readonly Action<Exception>  _handler;

        public ErrorProofEnumerable(IEnumerable<TType> enumerable, Action<Exception> handler)
        {
            _enumerable = enumerable;
            _handler    = handler;
        }

        public IEnumerator<TType> GetEnumerator() => new Enumerator(_enumerable.GetEnumerator(), _handler);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}