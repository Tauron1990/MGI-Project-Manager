#region

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>The new.</summary>
    [PublicAPI]
    public static class Factory
    {
        public static void Update(object toBuild)
        {
            if (toBuild == null) throw new ArgumentNullException(nameof(toBuild));
            var errorTracer = new ErrorTracer();

            CommonApplication.Current.Container.BuildUp(toBuild, errorTracer);
            if (errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The object.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <typeparam name="TObject">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TObject" />.
        /// </returns>
        [NotNull]
        public static TObject Object<TObject>([NotNull] params object[] args)
            where TObject : class
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var tracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(typeof(TObject), tracer, new BuildParameter[0], args);
            if (tracer.Exceptional)
                throw new BuildUpException(tracer);

            return (TObject) val;
        }

        [NotNull]
        public static object Object([NotNull] Type targetType, [NotNull] params object[] args)
        {
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));
            if (args == null) throw new ArgumentNullException(nameof(args));
            var errorTracer = new ErrorTracer();

            var val = CommonApplication.Current.Container.BuildUp(targetType, errorTracer, new BuildParameter[0], args);
            if (errorTracer.Exceptional)
                throw new BuildUpException(errorTracer);

            return val;
        }

        #endregion
    }
}