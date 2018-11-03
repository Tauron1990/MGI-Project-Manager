#region

using System;
using Castle.DynamicProxy;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The constructor policy.</summary>
    public class ConstructorPolicy : IPolicy
    { 
        /// <summary>Gets or sets the constructor.</summary>
        /// <value>The constructor.</value>
        public Func<IBuildContext, ProxyGenerator, object> Constructor { get; set; }

        public ProxyGenerator Generator { get; set; }

    }
}