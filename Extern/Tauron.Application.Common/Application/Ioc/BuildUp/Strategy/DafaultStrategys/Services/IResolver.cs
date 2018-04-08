using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The Resolver interface.</summary>
    public interface IResolver
    {
        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        object Create(ErrorTracer errorTracer);

        #endregion
    }
}