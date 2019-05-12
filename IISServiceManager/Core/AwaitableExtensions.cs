﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace IISServiceManager.Core
{
    public static class AwaitableExtensions
    {
        static public ConfiguredTaskAwaitable PrintContext(this ConfiguredTaskAwaitable t, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            PrintContext(callerName, line);
            return t;
        }

        static public Task PrintContext(this Task t, [CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            PrintContext(callerName, line);
            return t;
        }

        static private void PrintContext([CallerMemberName] string callerName = null, [CallerLineNumber] int line = 0)
        {
            var ctx = SynchronizationContext.Current;
            if (ctx != null)
            {
                Console.WriteLine("{0}:{1:D4} await context will be {2}:", callerName, line, ctx);
                Console.WriteLine("    TSCHED:{0}", TaskScheduler.Current);
            }
            else
            {
                Console.WriteLine("{0}:{1:D4} await context will be <NO CONTEXT>", callerName, line);
                Console.WriteLine("    TSCHED:{0}", TaskScheduler.Current);
            }
        }
    }
}