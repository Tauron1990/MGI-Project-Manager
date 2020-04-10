using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class CriticalExceptions
    {
        public static bool IsCriticalApplicationException([NotNull] this Exception ex)
        {
            ex = Unwrap(ex);
            return ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException
                   || ex is SecurityException;
        }


        public static bool IsCriticalException([NotNull] this Exception ex)
        {
            ex = Unwrap(ex);
            return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException
                   || ex is ThreadAbortException || ex is SEHException || ex is SecurityException;
        }


        [NotNull]
        public static Exception Unwrap([NotNull] this Exception ex)
        {
            while (ex.InnerException != null && ex is TargetInvocationException) ex = ex.InnerException;

            return ex;
        }
    }
}