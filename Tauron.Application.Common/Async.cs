using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class Async
    {
        public static Task StartNew([NotNull] this Action method) 
            => Task.Factory.StartNew(method);

        public static Task StartNewLong([NotNull] this Action method) 
            => Task.Factory.StartNew(method, TaskCreationOptions.LongRunning);
    }
}