using System;
using System.Threading.Tasks;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    internal sealed class UISyncFace : IUISynchronize
    {
        public Task BeginInvoke(Action action) => CommonApplication.QueueWorkitemAsync(action, false);
        public Task<TResult> BeginInvoke<TResult>(Func<TResult> action)
        {
            return (Task<TResult>)CommonApplication.QueueWorkitemAsync(action, false);
        }

        public void Invoke(Action action) => action();

        public TReturn Invoke<TReturn>(Func<TReturn> action) => action();
    }
}