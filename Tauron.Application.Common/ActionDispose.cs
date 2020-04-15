using System;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public sealed class ActionDispose : IDisposable
    {
        private readonly Action _action;

        public ActionDispose(Action action) => _action = action;

        public void Dispose() => _action();
    }
}