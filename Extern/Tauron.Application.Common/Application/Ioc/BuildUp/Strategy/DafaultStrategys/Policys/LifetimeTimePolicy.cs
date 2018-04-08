#region

using System;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The lifetime time policy.</summary>
    public sealed class LifetimeTimePolicy : IPolicy
    {
        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the lifetime context.</summary>
        /// <value>The lifetime context.</value>
        public ILifetimeContext LifetimeContext { get; set; }

        /// <summary>Gets or sets the liftime type.</summary>
        /// <value>The liftime type.</value>
        public Type LiftimeType { get; set; }

        /// <summary>Gets or sets a value indicating whether share liftime.</summary>
        /// <value>The share liftime.</value>
        public bool ShareLiftime { get; set; }

        #endregion
    }
}