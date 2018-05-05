using System;
using System.Threading;

namespace Tauron.Application.ProjectManager.Generic
{
    public sealed class ResultAwaiter<TResult> : IDisposable
    {
        private TResult _result;
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public TimeSpan? TimeOut { get; set; }

        public TResult DefaultValue { get; set; }

        public TResult Result
        {
            get
            {
                if (TimeOut == null) return _result;
                if (!_resetEvent.WaitOne(TimeOut.Value))
                    return DefaultValue;
                
                _resetEvent.WaitOne();
                return _result;
            }
        }

        public void SetResult(TResult result)
        {
            _result = result;
            _resetEvent.Set();
        }

        public void Reset()
        {
            _resetEvent.Reset();
            _result = default(TResult);
        }

        public void Dispose() => _resetEvent?.Dispose();
    }
}