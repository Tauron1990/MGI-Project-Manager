using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
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
        public static TType NotNull<TType>(TType? toCheck, string parameterName)
            where TType : class
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
        public static string NotNull(string toCheck, string parameterName)
        {
            Check(() => string.IsNullOrWhiteSpace(toCheck) ? new ArgumentNullException(parameterName) : null);
            return toCheck;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Check(bool toCheck, Func<Exception> exceptionBuilder)
        {
            Check(() => toCheck ? exceptionBuilder() : null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [NotNull]
        public static TValue CheckResult<TValue>(TValue? value, string name)
            where TValue : class
        {
            Check(() => value == null ? new ArgumentNullException(name) : null);
            return value!;
        }
    }
}