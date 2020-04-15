using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Tauron
{
    [JetBrains.Annotations.PublicAPI]
    [DebuggerStepThrough]
    public static class Argument
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(Func<Exception?> toCheck)
        {
            var ex = toCheck();

            if (ex == null) return;
            throw ex;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [return:NotNull]
        public static TType NotNull<TType>([NotNull]TType toCheck, string parameterName)
        {
            Check(() => toCheck == null ? new ArgumentNullException(parameterName) : null);
            return toCheck!;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TType NotNull<TType>(TType toCheck, string parameterName, string message)
        {
            Check(() => toCheck == null ? new ArgumentNullException(parameterName, message) : null);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string NotNull(string? toCheck, string parameterName)
        {
            Check(() => string.IsNullOrWhiteSpace(toCheck) ? new ArgumentNullException(parameterName) : null);
            return toCheck!;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(bool toCheck, Func<Exception> exceptionBuilder)
        {
            Check(() => toCheck ? exceptionBuilder() : null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [return:NotNull]
        public static TValue CheckResult<TValue>([NotNull]TValue value, string name)
        {
            Check(() => value == null ? new ArgumentNullException(name) : null);
            return value!;
        }
    }
}