#region

using System.Collections.Generic;
using Castle.DynamicProxy;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The interception policy.</summary>
    public class InterceptionPolicy : IPolicy
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterceptionPolicy" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InterceptionPolicy" /> Klasse.
        ///     Initializes a new instance of the <see cref="InterceptionPolicy" /> class.
        /// </summary>
        public InterceptionPolicy()
        {
            MemberInterceptor = new List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>>();
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the intercept attribute.</summary>
        /// <value>The intercept attribute.</value>
        public InterceptAttribute InterceptAttribute { get; set; }

        /// <summary>Gets the member interceptor.</summary>
        /// <value>The member interceptor.</value>
        public List<KeyValuePair<MemberInterceptionAttribute, IInterceptor>> MemberInterceptor { get; }

        #endregion
    }
}