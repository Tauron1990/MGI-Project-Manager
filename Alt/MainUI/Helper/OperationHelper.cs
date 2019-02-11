using System;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.Helper
{
    public sealed class OperationHelper<TParm, TResult>
        where TParm : class, IEquatable<TParm>
    {
        private readonly Func<TParm> _parm;
        private readonly Func<TParm, TResult> _action;
        private readonly Action<TResult> _resultAction;

        private bool _running;
        private bool _run;

        private readonly object _startLock = new object();

        private TParm _currentParm;
        private TParm _snapShot;

        public OperationHelper(Func<TParm> parm, Func<TParm, TResult> action, Action<TResult> resultAction)
        {
            _parm = parm;
            _action = action;
            _resultAction = resultAction;
        }

        private TResult Result { get; set; }

        public void Run()
        {
            lock (_startLock)
            {
                _currentParm = _parm();
                if (_running)
                {
                    if(!_currentParm.Equals(_snapShot))
                        _run = true;       
                }
                else
                {
                    _running = true;
                    _snapShot = _currentParm;
                    Task.Run(new Action(Runner));
                }
            }
        }

        private void Runner()
        {
            bool restart;

            do
            {
                Result = _action(_snapShot);

                lock (_startLock)
                {
                    restart = _run;
                    _run = false;

                    if (restart) continue;

                    Task.Run(() => _resultAction(Result));
                    _running = false;
                }

            } while (restart);
        }
    }
}