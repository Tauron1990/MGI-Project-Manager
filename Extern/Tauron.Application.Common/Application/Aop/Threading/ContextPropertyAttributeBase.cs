#region

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The context property attribute base.</summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ContextPropertyAttributeBase : ObjectContextPropertyAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ContextPropertyAttributeBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="ContextPropertyAttributeBase" /> class.
        /// </summary>
        protected ContextPropertyAttributeBase()
        {
            HolderName = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the holder name.</summary>
        /// <value>The holder name.</value>
        [NotNull]
        public string HolderName { get; set; }

        #endregion

        #region Fields

        #endregion
    }
}