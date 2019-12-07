using System;
using System.Threading.Tasks;

namespace Tauron.Application.Pipes.Threading
{
    internal class Worker
    {
        public event WorkerSucceededEventHandler Succeeded;
        public event WorkerExceptionEventHandler Error;


        public async void DoWork(Func<Task> action)
        {
            await DoWorkImpl(action);
        }

        private async Task DoWorkImpl(Func<Task> action)
        {
            try
            {
                await action();
                Callback(Succeed);
            }
            catch (Exception e)
            {
                Callback(() => Fail(e));
            }
        }

        private async Task Succeed()
        {
            if (Succeeded != null)
                await Succeeded();
        }

        private async Task Fail(Exception exception)
        {
            if (Error != null)
                await Error(exception);
        }

        private async void Callback(Func<Task> action)
        {
            await action();
        }
    }

    internal delegate Task WorkerSucceededEventHandler();

    internal delegate Task WorkerExceptionEventHandler(Exception exception);
}