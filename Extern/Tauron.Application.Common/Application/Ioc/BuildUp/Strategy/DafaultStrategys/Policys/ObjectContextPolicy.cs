#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The object context policy.</summary>
    public class ObjectContextPolicy : IPolicy
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectContextPolicy" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ObjectContextPolicy" /> Klasse.
        ///     Initializes a new instance of the <see cref="ObjectContextPolicy" /> class.
        /// </summary>
        public ObjectContextPolicy()
        {
            ContextPropertys = new List<Tuple<ObjectContextPropertyAttribute, MemberInfo>>();
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the context name.</summary>
        /// <value>The context name.</value>
        public string ContextName { get; set; }

        /// <summary>Gets or sets the context propertys.</summary>
        /// <value>The context propertys.</value>
        public List<Tuple<ObjectContextPropertyAttribute, MemberInfo>> ContextPropertys { get; }

        #endregion
    }
}