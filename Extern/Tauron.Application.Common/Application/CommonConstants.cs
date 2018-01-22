using System;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    public static class CommonConstants
    {
        public const string CommonCategory = "Tauron.Application.Common";

        [PublicAPI]
        [StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] params object[] parmsObjects)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (parmsObjects == null) throw new ArgumentNullException(nameof(parmsObjects));

            var logger = LogManager.GetLogger(CommonCategory, typeof(CommonConstants));

            var realMessage = parmsObjects.Length == 0 ? format : string.Format(format, parmsObjects);
            logger.Log(isError ? LogLevel.Error : LogLevel.Warn, realMessage, parmsObjects);
        }
    }
}